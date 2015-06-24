using UnityEngine;
using System.Collections;

public class Base : MonoBehaviour {
	
	public Transform MainCam;
	public Transform player;
	static int S = 26; // Denotes size of our square maze
	static wentdown[] is_down = new wentdown[S * S];
	static wentdown Y, N, temp;
	static bool yes = true, no = false;
	private AudioSource[] au;
	
	class wentdown // It will be helpful while using the go_down function to keep a track ...
	{              // ...of whether a cell has a downway path.  
		public bool reply{ get; set;}
	}
	
	class set_i{  // Indicator of each set: Tells the set index and whether is has go_down path.
		public int index{ get; set;}
		public set_i(int index){
			this.index = index;
		}
	}
	
	class Toggle{
		public bool val {
			get;
			set;
		}
		public Toggle(bool val){
			this.val = val;
		}
	}
	
	#region Utility structs
	struct cell{//Representing each block in the grid
		public bLine a_bline, b_bline, l_bline, r_bline;
		public set_i set;
		public int cell_count;
		public bool break_wall;
	}
	
	struct bLine{//These baselines will form a base whose toggle values will...   
		public GameObject Baseline;//...decide whether a wall is to be placed upon them.
		public Toggle toggle;
		public GameObject wall;
	}
	
	struct Grid{//Representing the whole of the grid
		public bLine[,] blineH, blineV;
	}
	
	struct switch_{
		public int index;
		public GameObject Switch;
		public bool trigger;
		public Ray det;
	}
	#endregion
	
	#region Construct a WALL
	//Function to create a single wall
	GameObject formWall (Vector3 pos, Quaternion rot)
	{
		GameObject wall = GameObject.CreatePrimitive (PrimitiveType.Cube);
		float y = Random.Range (30f, 40f);
		wall.transform.localScale = new Vector3 (20f, y, 3f);
		wall.transform.localRotation = rot;
		wall.transform.position = pos;
		Rigidbody w = wall.AddComponent<Rigidbody> ();
		wall.tag = "wall";
		w.isKinematic = true;
		Renderer r = wall.GetComponent<Renderer> ();
		r.material = (Material)Resources.Load ("New Material", typeof(Material));
		return wall;
	}
	#endregion
	
	#region Single BaseLine
	//To create a single BaseLine given its position and rotation
	GameObject formBline (Vector3 pos, Quaternion rot)
	{
		GameObject bline = GameObject.CreatePrimitive (PrimitiveType.Plane);
		bline.transform.position = pos;
		bline.transform.localRotation = rot;
		bline.transform.localScale = new Vector3 (.01f, .01f, .01f);
		return bline;
	}
	#endregion
	
	#region Partial Grid 
	// Method to create Horizontal/Vertical grid lines and walls
	bLine[,] partialGrid (int rows, int col)
	{
		
		bLine[,] pgrid = new bLine[rows, col];//multiD array of GO in which output array will be stored implies that grid-V has to be formed 
		
		if (rows < col) {//grid-V
			for (int i=0; i<rows; i++) {
				for (int j=0; j<col; j++) {
					pgrid [i, j].toggle = new Toggle (true);
					Vector3 pos = new Vector3 (-10 * (float)S/2 + 20 * j, 35f, 5f * S - 20 * (i+1)+10);
					pgrid [i, j].Baseline = formBline (pos, Quaternion.Euler (0f, 90f, 0f));
					pgrid [i, j].toggle.val = true;
					pgrid [i, j].wall = formWall (pos, Quaternion.Euler (0f, 90f, 0f));// Instead of iterating through new loops we shall construct walls and toggle on/off values on BLines simultaneuosly.
				}	
			}
		} else {//grid-H
			for (int i=0; i<rows; i++) {
				for (int j=0; j<col; j++) {
					pgrid [i, j].toggle = new Toggle (true);
					Vector3 pos = new Vector3 (-5f * S + 20 * (j+1)-10, 35f, 5f * S - 20 * i);
					pgrid [i, j].Baseline = formBline (pos, Quaternion.identity);
					pgrid [i, j].toggle.val = true;
					pgrid [i, j].wall = formWall (pos + Vector3.up, Quaternion.identity);
				}
			}
		}
		return pgrid;
	}
	#endregion
	
	#region Construct Grid 
	// Completing Grid
	Grid ConstructGrid ()
	{
		Grid g;
		g.blineH = partialGrid (S+1, S);
		g.blineV = partialGrid (S, S+1);
		return g;
	}
	#endregion
	
	#region Cell Creation
	void assign_cells (cell[,] cells, Grid g)
	{
		int cell_count = 1;  // To keep a count of the no. of cells and assign them their indices accordingly
		for (int i=0; i<S; i++) {
			for (int j=0; j<S; j++, cell_count++) {
				cells [i, j].a_bline = g.blineH [i, j];
				cells [i, j].b_bline = g.blineH [i + 1, j];
				cells [i, j].l_bline = g.blineV [i, j];
				cells [i, j].r_bline = g.blineV [i, j + 1];
				cells [i, j].set = new set_i(cell_count);
				cells [i, j].cell_count = cell_count;
				cells [i, j].break_wall = true;
				is_down[cell_count-1] = new wentdown();
				is_down[cell_count-1].reply = false;
			}
		}
	}
	#endregion
	
	#region Merging Cells
	// Function to merge any 2 cells given their line of intersection
	void merge_cells (cell cell1, cell cell2, bLine comm_bline, cell[,] cells)
	{
		int i1 = cell1.set.index, i2 = cell2.set.index;
		if (i1 > i2)
			merge_cells (cell2, cell1, comm_bline, cells);
		else {
			if(comm_bline.toggle.val && cell1.break_wall){
				Destroy(comm_bline.Baseline);
				Destroy(comm_bline.wall);
				comm_bline.toggle.val = false;
			}
			cell2.break_wall = false;
			if(cell2.set.index != cell1.set.index){
				cell2.set.index = i1;
				int N = cell2.cell_count;
				int i, j;
				i = (N-1) / S;
				if(N % S != 0) j = N % S - 1;
				else j = S - 1;
				if(i > 0){
					if(cells[i-1, j].set.index == i2) merge_cells(cell2, cells[i-1, j], cell2.a_bline, cells);
				}
				if(j < S-1){
					if(cells[i, j+1].set.index == i2) merge_cells(cell2, cells[i, j+1], cell2.r_bline, cells);
				}
				if(j > 0){
					if(cells[i, j-1].set.index == i2) merge_cells(cell2, cells[i, j-1], cell2.l_bline, cells);
				}
				if(i < S-1){
					if(cells[i+1, j].set.index == i2) merge_cells(cell2, cells[i+1, j], cell2.b_bline, cells);
				}
				cell2.break_wall = true;
			}
			//			System.String s = string.Format("{0}, {1} : {2}", i, j, is_down[cells[i,j].set.index - 1].reply);
			//			Debug.Log(s);
		}
	}
	#endregion
	
	#region Merge Row
	// Randomly merging any sets of a given row.
	void merge_row (int row, cell[,] cells)
	{
		for (int i=0; i<S-1; i++) {
			if (Random.Range (0, Random.Range(1,3)) == 0) {
				merge_cells (cells [row, i], cells [row, i + 1], cells [row, i].r_bline, cells);
			}
		}
	}
	#endregion
	
	#region Going Down
	// Function to randomly merge a cell from a given row with the one just below it
	void go_down (int row, cell[,] cells)
	{
		if (Y.reply == no) {
			temp = Y;
			N.reply = yes;
			Y = N;
			N = temp;
			temp = null;
		} else {
			Y.reply = true;
			N.reply = false;
		}
		
		int i;
		for (i=0; i<S; i++) {
			//if (is_down [cells [row, i].set.index - 1] == N) {
			if (Random.Range (0, Random.Range (3, 5)) == 0) {
				merge_cells (cells [row, i], cells [row + 1, i], cells [row, i].b_bline, cells);
				is_down [cells [row, i].set.index - 1] = Y;
			}
			//}
		}
		
		//Checking if any set is left without being given a downward path
		for (i=0; i<S; i++) {
			if (!is_down[cells [row, i].set.index - 1].reply) {
				merge_cells (cells [row, i], cells [row + 1, i], cells [row, i].b_bline, cells);
				is_down[cells [row, i].set.index - 1] = Y;
			}
		}
		
		Y.reply = no;
	}
	#endregion
	
	#region Concluding Statements
	// Concluding the maze in the last row by checking if any cells belong to different sets
	void conclude (cell[,] cells)
	{
		for (int i=0; i<S-1; i++) {
			if (cells [S-1, i].set.index != cells [S-1, i + 1].set.index) {
				merge_cells (cells [S-1, i], cells [S-1, i + 1], cells [S-1, i].r_bline, cells);
			}
		}
	}
	#endregion
	
	#region Generate The MAZE
	// Generate The MAZE
	void MazeGeneration (cell[,] cells)
	{
		int i;
		for (i=0; i<S-1; i++) {
			merge_row (i, cells);
			go_down (i, cells);
		}
		conclude (cells);
	}
	#endregion
	
	#region Destroy Maze
	void DestroyMaze (Grid g)
	{
		int i, j;
		for (i=0; i<S+1; i++) {
			for (j=0; j<S; j++) {
				Destroy (g.blineH [i, j].Baseline);
				Destroy (g.blineV [j, i].Baseline);
				Destroy (g.blineH [i, j].wall);
				Destroy (g.blineV [j, i].wall);
			}
		}
	}
	#endregion
	
	#region Create a Switch
	switch_ create_switch_at (cell cell, int ind)
	{
		Transform t = cell.a_bline.Baseline.transform;;
		switch_ s;
		s.Switch = GameObject.CreatePrimitive (PrimitiveType.Cube);
		Rigidbody r = s.Switch.AddComponent<Rigidbody> ();
		s.Switch.transform.position = t.position - t.forward * 3 + t.up * 6;
		s.Switch.transform.localScale = new Vector3 (1.5f, 2, .5f);
		r.useGravity = false;
		r.isKinematic = true;
		s.trigger = false;
		s.index = ind;
		
		GameObject rod = GameObject.CreatePrimitive (PrimitiveType.Cube);
		rod.transform.parent = s.Switch.transform;
		rod.transform.localPosition = -1.5f * rod.transform.up;
		rod.transform.localScale = new Vector3 (.25f, 3, .25f);
		rod.GetComponent<Renderer> ().material.color = Color.grey;
		
		GameObject button = GameObject.CreatePrimitive (PrimitiveType.Cylinder);
		button.transform.parent = s.Switch.transform;
		button.transform.localPosition = -.5f * s.Switch.transform.forward;
		button.transform.localScale = new Vector3 (.5f, .5f, .5f);
		button.transform.localRotation = Quaternion.Euler(-90, 0, 0);
		button.GetComponent<Renderer> ().material.color = Color.red;
		
		Light light = s.Switch.AddComponent<Light> ();
		light.type = LightType.Point;
		//light.areaSize = new Vector2 (10, 10);
		light.intensity = 5;
		light.color = Color.red;
		light.range = 20;
		
		s.det = new Ray(button.transform.position, new Vector3(0, 0, -1));
		
		return s;
	}
	#endregion
	
	#region Placing the switches
	void put_all_switches (cell[,] cells, switch_[] S_)
	{
		int[] switches = new int[4];
		int i, j;
		switches [0] = Random.Range (1, S * S + 1);
		switches [1] = (switches [0] + 1449) % (S * S);
		switches [2] = (switches [1] + 1449) % (S * S);
		int[] temp = new int[4];
		temp [0] = 1;
		temp [1] = S;
		temp [2] = S * S - S + 1;
		temp [3] = S * S;
		for (i=0; i<4; i++) {
			for (j=0; j<3; j++) {
				if (temp [i] == switches [j])
					break;
				else if (j == 2)
					switches [3] = temp [i];
			}
		}
		int r, c;
		for (i=0; i<4; i++) {
			int N = switches[i];
			r = (N-1) / S;
			if(N % S != 0) c = N % S - 1;
			else c = S - 1;
			S_[i] = create_switch_at(cells[r, c], i);
		}
	}
	#endregion
	
	#region |vector|
	float mag (Vector3 v1, Vector3 v2)
	{
		return Vector3.Magnitude (v2 - v1);
	}
	#endregion
	
	
	static Grid grid;
	static cell[,] cells;
	static switch_ [] Switches;
	static bool exit;
	static int a;
	
	void Start () {
		Y = new wentdown ();
		Y.reply = yes;
		N = new wentdown ();
		N.reply = no;
		timer = 0.0f;
		transform.localScale = new Vector3 (500, 1, 500);// Size and Location of Base Plate
		transform.localPosition = new Vector3 (50, 10, -50);
		
		au = GetComponents<AudioSource> ();
		
		grid =  ConstructGrid ();// Wireframe and Wall creation
		
		cells = new cell [S, S];// Matrix of empty cells
		assign_cells (cells, grid);// Filling of cells
		
		MazeGeneration(cells);// Generation of Maze
		
		Switches = new switch_[4];
		put_all_switches (cells, Switches);// Placing 'turned off' Switches
		
		exit = false;
	}
	IEnumerator go_to_main_menu(float waitTime){
		yield return new WaitForSeconds (waitTime);
		Application.LoadLevel(4);

	}
	
	private float timer = 0;
	static switch_ lastSwitch;
	static float intensity = 8;
	private int number = 4;
	//private int timer1 =0, timer2 =5  ;
	void OnGUI(){
		GUI.Box(new Rect (10, 45, 150, 25), "Switches Left:"+ number.ToString ("0") );
////		//new Rect (10, 45, 150, 25), + number.ToString ("0")
////		GUILayout.Label ("Switches Left:" );
//		if (timer2 >= 0) {
//
//			GUI.Box (new Rect (500, 300, 500, 30), " Seek for another flickering switch which will in turn open the gates.. Run !!");
//			timer2 = timer2 - 1;
//		}
//
	}
	
	void Update () {
		if (Input.GetKey (KeyCode.Escape)) {
			//Time.timeScale = 0;
			Application.LoadLevel(3);
		}

		if (timer >= 25) {
			if( !au[2].isPlaying && timer <= 29.5f  && Time.timeScale == 1 ) au[2].Play();
			if(timer >= 30){
				DestroyMaze(grid);
				grid =  ConstructGrid ();
				assign_cells (cells, grid); 
				MazeGeneration(cells);
				timer = 0;
			}
		}
		timer = timer + Time.deltaTime;
		
		if (Time.time >= 270 && !au [2].isPlaying  && Time.timeScale == 1 ) {
			au[2].Play();
		}
		
		RaycastHit hit;
		
		
		// Checking if player is in availing range of switch and whether he has pressed LCtrl to turn the switch on
		if (!Switches[0].trigger && mag (player.position, Switches [0].Switch.transform.position) <= 20) {
			
			if(Input.GetKeyDown(KeyCode.LeftControl) && Physics.Raycast(Switches[0].det, out hit, 10) && Time.timeScale == 1 ){
				Switches[0].trigger = true;
				Switches[0].Switch.GetComponent<Light>().color = Color.green;
				Switches[0].Switch.GetComponentsInChildren<Renderer>()[2].material.color = Color.green;
				au[0].pitch = Random.Range(.8f, 1.2f);
				au[0].Play();
				number -= 1;

			}
		}
		if (!Switches[1].trigger && mag (player.position, Switches [1].Switch.transform.position) <= 20) {
			
			if(Input.GetKeyDown(KeyCode.LeftControl) && Physics.Raycast(Switches[1].det, out hit, 10) && Time.timeScale == 1 ){
				Switches[1].trigger = true;
				Switches[1].Switch.GetComponent<Light>().color = Color.green;
				Switches[1].Switch.GetComponentsInChildren<Renderer>()[2].material.color = Color.green;
				au[0].pitch = Random.Range(.8f, 1.2f);
				au[0].Play();
				number -= 1;

			}
		}
		if (!Switches[2].trigger && mag (player.position, Switches [2].Switch.transform.position) <= 20) {
			
			if(Input.GetKeyDown(KeyCode.LeftControl) && Physics.Raycast(Switches[2].det, out hit, 10) && Time.timeScale == 1 ){
				Switches[2].trigger = true;
				Switches[2].Switch.GetComponent<Light>().color = Color.green;
				Switches[2].Switch.GetComponentsInChildren<Renderer>()[2].material.color = Color.green;
				au[0].pitch = Random.Range(.8f, 1.2f);
				au[0].Play();
				number -= 1;

			}
		}
		if (!Switches[3].trigger && mag (player.position, Switches [3].Switch.transform.position) <= 20) {
			
			if(Input.GetKeyDown(KeyCode.LeftControl) && Physics.Raycast(Switches[3].det, out hit, 10)  && Time.timeScale == 1 ){
				Switches[3].trigger = true;
				Switches[3].Switch.GetComponent<Light>().color = Color.green;
				Switches[3].Switch.GetComponentsInChildren<Renderer>()[2].material.color = Color.green;
				au[0].pitch = Random.Range(.8f, 1.2f);
				au[0].Play();
				number -= 1;

			}
		}
		
		// Checking if all switches have been activated so as to trigger on the exit switch
//		if (Switches [0].trigger && Switches [1].trigger && Switches [2].trigger && Switches [3].trigger && !exit) {
//			a = Random.Range(1, S-1);
//			Transform t = cells[0, a].a_bline.Baseline.transform;
//			lastSwitch.Switch = GameObject.CreatePrimitive(PrimitiveType.Cube);
//			lastSwitch.Switch.GetComponent<Renderer>().material.color = Color.white;
//			GameObject button = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
//			button.transform.rotation = Quaternion.Euler(-90, 0, 0);
//			button.transform.position = t.position - t.forward * 5.1f + t.up * 7;
//			button.GetComponent<Renderer>().material.color = Color.red;
//			button.transform.localScale = new Vector3(1, .2f, 1);
//			lastSwitch.Switch.transform.localScale = new Vector3(1.5f, 2f, .2f);
//			lastSwitch.Switch.transform.position = t.position - t.forward * 5 + t.up * 7;
//			Light l = lastSwitch.Switch.AddComponent<Light>();
//			GameObject rod = GameObject.CreatePrimitive (PrimitiveType.Cube);
//			rod.transform.parent = lastSwitch.Switch.transform;
//			rod.transform.localPosition = -1.5f * rod.transform.up;
//			rod.transform.localScale = new Vector3 (.25f, 3, .25f);
//			rod.GetComponent<Renderer> ().material.color = Color.grey;
//			l.intensity = 0;
//			l.range = 50;
//			l.color = Color.yellow;
//
//			Destroy(Switches[0].Switch);
//			Switches[0].trigger = true;
//
//			Destroy(Switches[1].Switch);
//			Switches[1].trigger = true;
//		
//			Destroy(Switches[2].Switch);
//    		Switches[2].trigger = true;
//
//			Destroy(Switches[3].Switch);
//			Switches[3].trigger = true;
//			
//			exit = true;
//
//		}
		if (Switches [0].trigger && Switches [1].trigger && Switches [2].trigger && Switches [3].trigger && !exit) {
			a = Random.Range(1, S-1);
			Transform t = cells[0, a].a_bline.Baseline.transform;
			lastSwitch.Switch = GameObject.CreatePrimitive(PrimitiveType.Cube);
			lastSwitch.Switch.GetComponent<Renderer>().material.color = Color.white;
			GameObject button = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
			
			button.transform.parent = lastSwitch.Switch.transform;
			button.transform.localPosition = -.5f * lastSwitch.Switch.transform.forward;
			button.transform.localScale = new Vector3 (.5f, .5f, .5f);
			button.transform.localRotation = Quaternion.Euler(-90, 0, 0);
			button.GetComponent<Renderer> ().material.color = Color.red;
			
			lastSwitch.Switch.transform.localScale = new Vector3(1.5f, 2f, .2f);
			lastSwitch.Switch.transform.position = t.position - t.forward * 5 + t.up * 7;
			Light l = lastSwitch.Switch.AddComponent<Light>();
			GameObject rod = GameObject.CreatePrimitive (PrimitiveType.Cube);
			rod.transform.parent = lastSwitch.Switch.transform;
			rod.transform.localPosition = -1.5f * rod.transform.up;
			rod.transform.localScale = new Vector3 (.25f, 3, .25f);
			rod.GetComponent<Renderer> ().material.color = Color.grey;
			
			l.intensity = 0;
			l.range = 50;
			l.color = Color.yellow;
			
			Destroy(Switches[0].Switch);
			Switches[0].trigger = true;
			
			Destroy(Switches[1].Switch);
			Switches[1].trigger = true;
			
			Destroy(Switches[2].Switch);
			Switches[2].trigger = true;
			
			Destroy(Switches[3].Switch);
			Switches[3].trigger = true;
			
			exit = true;
		}

		
		// Command when exit switch has been triggered on 
		if (exit) {
			
			Light l = lastSwitch.Switch.GetComponent<Light>();
			if(l.intensity < .5f) intensity = 8;
			else if(l.intensity > 7.5f) intensity = 0; 
			l.intensity = Mathf.Lerp(l.intensity, intensity, .2f);
			
			if (mag (player.position, lastSwitch.Switch.transform.position) <= 10) {
				
				if (Input.GetKeyDown (KeyCode.LeftControl)  && Time.timeScale == 1 ) {
					lastSwitch.trigger = true;
					//lastSwitch.Switch.GetComponent<Light>().color = Color.green;
					lastSwitch.Switch.GetComponentsInChildren<Renderer>()[1].material.color = Color.green;
					Destroy (grid.blineH [0, a].Baseline);
					grid.blineH [0, a].toggle.val = false;
					Rigidbody r = grid.blineH [0, a].wall.GetComponent<Rigidbody> ();
					r.useGravity = true;
					r.isKinematic = false;
					//r.mass = 10000;
					r.AddForce(Vector3.down * 100 * Time.deltaTime);
					lastSwitch.Switch.GetComponent<Light>().color = Color.black;
					au[2].volume = 0;
					au[1].Play();
					StartCoroutine(go_to_main_menu(6.2f));

				}
			}
			if(!grid.blineH [0, a].wall.GetComponent<Rigidbody> ().isKinematic) grid.blineH [0, a].wall.GetComponent<Rigidbody> ().AddForce(Vector3.down * 100 * Time.deltaTime);
		}
	}
}







