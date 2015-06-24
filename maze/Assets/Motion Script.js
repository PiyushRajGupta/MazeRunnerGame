var Controller ; //
var v ;
var h ;
var sprint ;

function Start () {
    Controller = GetComponent (Controller) () ;
}

function Update () {
  v = Input.GetAxis("Vertical");
  //h = Input.GetAxis("Horizontal");
  //Sprinting ();

}

function FixedUpdate () {
Controller.SetFloat("Walk", v);
//Controller.SetFloat("Turn", h);
//Controller.SetFloat("Sprint", sprint) ;

}

function Sprinting() {

if(Input.GetButton("Fire1")) {
 sprint =0.2;
 }

 else {
   sprint =0.0;
   }
   
}