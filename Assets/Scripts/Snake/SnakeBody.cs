using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SnakeBody : MonoBehaviour
{
	// Components
	public SnakeBodyCell cellPrefab;

	// System
	private SnakeController snakeController;
	private SnakeSkeleton _skeleton;
	private SnakeBodyCell _head;
	private SnakeBodyCell _tail;
	private SnakeBodyCell firstEmptyCell;
	private SnakeBodyCell currentItemCell;
	private List<SnakeBodyCell> _cellList;
	[HideInInspector] public float correction = 0;
	private int growQueue = 0;
	private float growTime;


// UNITY METHODS ///////////////////////////////////////////////////////////////
	
	void Awake()
	{
		// Get snake controller reference
		snakeController = GetComponent<SnakeController>();

		// Get head cell reference
		_head = GetComponent<SnakeBodyCell>();
		_head.body = this;

		// Initialize body
		Init();
	}

	void Update()
	{
		// _skeleton.Draw();
			
		// // CHAIN DEBUG ///////////////////////////////////////////////////////////////
		// for( int i = 0; i < _cellList.Count; i++ )
		// {
		// 	SnakeBodyCell cell = _cellList[ i ];

		// 	float opacity = Mathf.Max( 1f - i/10f, 0.3f );

		// 	// Draw non-corrected chain
		// 	MyDraw.DrawCircle( Vector3.right * cell.relPos, 0.5f, Color.white * opacity );
		// 	// Draw corrected chain
		// 	MyDraw.DrawCircle( Vector3.right * (cell.relPos + correction), 0.5f, Color.cyan * opacity );
		// 	// Draw correction value
		// 	Debug.DrawLine( Vector3.zero, -Vector3.right * correction, Color.cyan );
		// }
		// //////////////////////////////////////////////////////////// EO CHAIN DEBUG //
	}

	void OnDrawGizmos()
	{
		// ITEM SLOTS DEBUG ///////////////////////////////////////////////////////////////
		foreach( SnakeBodyCell cell in cellList )
		{
			if( cell == firstEmptyCell )
				MyDraw.DrawCircle( cell.transform.position, 0.5f, Color.blue );

			if( cell == currentItemCell )
				MyDraw.DrawCircle( cell.transform.position, 0.3f, Color.red );
		}
		//////////////////////////////////////////////////////////// EO ITEM SLOTS DEBUG //
	}

	void OnGUI()
	{
		// GUI.Box( new Rect( 100, 0, 100, Screen.height ), ToString() );
		
		// if( GUI.Button( new Rect(200,0,100,20), "GROW" ) )
		// 	Grow();
	}
//////////////////////////////////////////////////////////// EO UNITY METHODS //

// INIT ///////////////////////////////////////////////////////////////

	public void Init()
	{
		// Reposition
		if( snakeController.respawnPoint != null ){	
			transform.position = snakeController.respawnPoint.position;
			transform.rotation = snakeController.respawnPoint.rotation;
		}

		// // Set Color of head
		// _head.GetComponent<Renderer>().material.color = snakeController.settings.color;

		// Create initial skeleton
		_skeleton = new SnakeSkeleton();
		skeleton.AppendJoint( transform.position + transform.forward * 0 );
		// skeleton.AppendJoint( transform.position + transform.forward * -snakeController.settings.lengthOnBorn * 1.1f );
		skeleton.AppendJoint( transform.position + transform.forward * -15f );

		// SETUP INIT CELLS ///////////////////////////////////////////////////////////////
		// 

		// Remove old body cells from scene
		if( _cellList != null )
			foreach( SnakeBodyCell cell in _cellList )
				if( !cell.isHead )
					Destroy( cell );

		// Create empty list
		_cellList = new List<SnakeBodyCell>();

		// Add head cell to the list
		_cellList.Add( _head );

		// Mark head cell as isHead
		_head.isHead = true;

		// Mark head cell as isTail
		_head.isTail = true;

		// Set tail reference
		_tail = _head;

		// Instantiate cells
		for( int i = 0; i < snakeController.settings.lengthOnBorn; i++ )
			InstantiateCell( true );
		//////////////////////////////////////////////////////////// EO SETUP INIT CELLS //
	}
//////////////////////////////////////////////////////////// EO INIT //

// OTHER METHODS ///////////////////////////////////////////////////////////////

	/**
	 * InstantiateCell
	 */
	public SnakeBodyCell InstantiateCell( bool fullyGrown = false )
	{
		// print("INSTANTIATE");
		// Instantiate cell
		SnakeBodyCell cell = (Instantiate( cellPrefab.transform, Vector3.zero, Quaternion.identity ) as Transform).GetComponent<SnakeBodyCell>();

		// // Set color to new cell
		// cell.SetColor( snakeController.settings.color );

		// Set body reference
		cell.body = this;

		// Unset old tail
		if( _tail != null )
			_tail.isTail = false;

		// Set new tail
		_tail = cell;
		_tail.isTail = true;

		// Set initial empty cell
		if( firstEmptyCell == null )
			firstEmptyCell = cell;

		// Set value to last cells value, plus combined buffers
		SnakeBodyCell prev = _cellList[ _cellList.Count - 1 ];
		
		// Decide where to spawn:
		// behind the last cell, or under it
		cell.relPos = fullyGrown ? prev.relPos + ( prev.buffer + cell.buffer ) : prev.relPos;

		// Put cell on the skeleton to it's relative position with correction
		PutOnSkeleton( cell.transform, cell.relPos + correction );

		// Keep list of all body cells
		_cellList.Add( cell );

		// Return newly created cell
		return cell;
	}

	/**
	 * 
	 */
	public void DestroyCell( SnakeBodyCell cell )
	{
		// Set new tail
		if( cell.isTail ){
			try {
				cell.previous.isTail = true;
				_tail = cell.previous;
			}
			catch( System.ArgumentException e ){ print(e); }
		}

		// Remove from list
		cellList.Remove( cell );

		// Remove object
		Destroy( cell.gameObject );
	}

	/**
	 * UpdateBody
	 */
	public void UpdateBody( float positionChange )
	{
		// Check if any cell needs to be grown
		CheckGrowing();

		// print("UPDATE");
		for( int i = 0; i < _cellList.Count; i++ )
		{
			SnakeBodyCell cell = _cellList[ i ];

			// UPDATE VALUES ///////////////////////////////////////////////////////////////
			if( cell.isHead )
			{
				// Move cell value towards skeleton beginning
				cell.relPos -= positionChange;

				// Update correction
				correction += positionChange;
			}
			else
			{
				// Get previous cell reference
				SnakeBodyCell prev = _cellList[ i - 1 ];
				
				// Distance between centers of cell and previous cell
				float distance = -(prev.relPos - cell.relPos);

				// Gap between edges of cell and previous cell
				float gap = distance - cell.buffer - prev.buffer;

				// Move cell forward towards skeleton beginning
				// for gap distance and consider bond strength (0 to 1)
				cell.relPos -= Mathf.Max( 0, gap ) * cell.bondStrength;

				// Put cell on the skeleton to it's relative position with correction
				PutOnSkeleton( cell.transform, cell.relPos + correction );
			}
			//////////////////////////////////////////////////////////// EO UPDATE VALUES //
		}

		// Trim exces part of the skeleton
		float realSnakeLength = tail.relPos + correction;
		skeleton.TrimEnd( Mathf.Max( Mathf.Min( realSnakeLength + 1, _skeleton.length ), 0 ) );
	}

	/**
	 * UpdateBodyShrink
	 */
	public void UpdateBodyShrink( float positionChange )
	{
		// print("UPDATE");
		for( int i = 0; i < _cellList.Count; i++ )
		{
			SnakeBodyCell cell = _cellList[ i ];

			// UPDATE VALUES ///////////////////////////////////////////////////////////////
			if( cell.isHead )
			{
				// Move cell value towards skeleton beginning
				cell.relPos -= positionChange;
			}
			else
			{
				// Get previous cell reference
				SnakeBodyCell prev = _cellList[ i - 1 ];
				
				// Distance between centers of cell and previous cell
				float distance = -(prev.relPos - cell.relPos);

				// Gap between edges of cell and previous cell
				float gap = distance - cell.buffer - prev.buffer;

				// Move cell forward towards skeleton beginning
				// for gap distance and consider bond strength (0 to 1)
				cell.relPos -= Mathf.Max( 0, gap ) * cell.bondStrength;
			}
				
			// Put cell on the skeleton to it's relative position with correction
			PutOnSkeleton( cell.transform, cell.relPos + correction );
			//////////////////////////////////////////////////////////// EO UPDATE VALUES //
		}
	}

	// Put given cell on on cell along given value
	public void PutOnSkeleton( Transform prefab, float distance )
	{
		// Get data where certain point on skeleton is absolute to world
		SkeletonPointData point = skeleton.GetPointOnSkeleton( distance );

		// Apply data
		prefab.position = point.position;
		prefab.rotation = point.rotation;
	}

	public void Grow( int num = 1 )
	{
		while( num-- > 0 )
		{
			// All cells are filled
			if( firstEmptyCell == null )
			{
				// Update grow queue
				growQueue ++;

				// Empty all items
				while( GetItem() ){}

				// Set initial empty cell
				firstEmptyCell = _head.next;
			}

			// There is still place for item
			else
			{
				// Mark cell filled
				firstEmptyCell.isItemOn = true;

				// Get cell reference from where to pull next item
				currentItemCell = firstEmptyCell;

				// Previously empty cell is filled now, so set next cell to be filled
				firstEmptyCell = firstEmptyCell.next;
			}
		}
	}

	// Remove item from body and return flag if it even existed
	public bool GetItem()
	{	
		// If any cell has item
		if( currentItemCell != null )
		{
			// Empty item
			currentItemCell.isItemOn = false;

			// Currently emptied cell is now first empty
			firstEmptyCell = currentItemCell;

			// Set new current item cell
			if( currentItemCell.previous.isHead == false )
				currentItemCell = currentItemCell.previous;
			else
				currentItemCell = null;

			return true;
		}
		return false;
	}

	private void CheckGrowing()
	{
		// Check for grow conditions
		if( growQueue > 0 && Time.time >= growTime )
		{
			// Instantiate cell
			InstantiateCell();

			// Update grow queue
			growQueue--;

			// Reset grow delay timer (wait before growing next cell)
			growTime = Time.time + snakeController.settings.growDelay;
		}
	}

	public void SetColor( Color color )
	{
		// Set color of body cells
		foreach( SnakeBodyCell cell in cellList )
			cell.SetColor( color );
	}
//////////////////////////////////////////////////////////// EO OTHER METHODS //

// GETTERS/SETTERS ///////////////////////////////////////////////////////////////

	public SnakeSkeleton skeleton {
		get{ return _skeleton; }
	}

	public SnakeBodyCell head {
		get{ return _head; }
	}

	public SnakeBodyCell tail {
		get{ return _tail; }
	}

	public List<SnakeBodyCell> cellList {
		get{ return _cellList; }
	}

	public int size {
		get{ return _cellList.Count; }
	}
//////////////////////////////////////////////////////////// EO GETTERS/SETTERS //

// DEBUG ///////////////////////////////////////////////////////////////
	override public string ToString()
	{
		string trace = "";

		// // Trace relative positions of cells
		// trace += "correction: " + correction + "\n";
		// for( int i = 0; i < _cellList.Count; i++ )
		// 	trace += "_cellList[" + i + "].relPos: " + _cellList[i].relPos + "\n";

		// Trace cell list
		foreach( SnakeBodyCell cell in cellList )
			trace += cell + "\n";

		return trace;
	}
//////////////////////////////////////////////////////////// EO DEBUG //
}
