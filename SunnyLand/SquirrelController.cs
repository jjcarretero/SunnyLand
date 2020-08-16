using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SquirrelController : MonoBehaviour
{
	//Una variable publica puede modificarse tanto desde otro script como desde Unity. 
	//Un float es un tipo de dato numérico. Es decir puedo almacenar números decimales. 
	public float Speed; //Esta variable es la velocidad de movimiento Izquierda-derecha.
	//Una nueva variable tipo float para la velocidad de salto. Recordemos que esta velocidad deberá tener valores más altos como 300 ó 500. 
	public float JumpSpeed;
	//Nueva variable para ponerle la velocidad a la que va agachado. 
	public float CrouchSpeed = 1f;

	//La variable Rigidbody contendrá el componente que tiene el jugador. Para especificar que es ese componente y no otro, tenemos que poner "rb = GetComponent<Rigidbody2D>();" en el start. 
	private Rigidbody2D rb;

	//Las capas nos permiten clasificar los objetos rápidamente. En esta capa pondremos los objetos que son suelo, para que el personaje entienda cuando esta apoyado en el suelo y cuando en el aire.
	public LayerMask Ground;
	//En esta capa pondremos tanto los objetos de suelo como las paredes, para evitar que el personaje se nos quede pegado en ellas. 
	public LayerMask WallMask;

	//Los puntos 1 y 2 generaran un área en la parte baja del personaje que será la que entre en contacto con el suelo para saber si esta tocando o no. 
	//El punto 1 será la esquina izquierda arriba de nuestro cuadrado y el punto 2 será la esquina abajo derecha de nuestro teclado. 
	public GameObject Point1, Point2;
	//Punto 3 y Punto 4 harán el rectangulo que detecta paredes.
	public GameObject Point3, Point4;


	//Una bool es un dato que solo puede ser verdadero o falso. Este en concreto nos dice si toca el suelo. True será SI toca el suelo. False será NO toca el suelo. 
	private bool b_IsGrounded = false;
	//Esta bool nos dice si está tocando alguna pared o no. 
	private bool b_WallDetect;
	//Esta bool nos dice si está mirando a la derecha o no.
	private bool b_LookRight;

	//Gracias al animator podemos intercambiar entre una animación y otra. 
	public Animator Anim;

	//Int es un tipo de dato numerico. Integer, en ingles. Numeros Enteros, 1, 2, 33...1024... 
	//En estas variables manejamos las puntuaciones tanto en el UI como en variable numerica
	private int Score = 0;
	public Text ScoreText;

	//Para la vida del personaje
	private int Lifes = 4;
	public Sprite[] HealthSprites;
	public Image Hud;

	//Esto es un extra que añadi despues para quitarle la cabeza cuando se agache y que pueda pasar por sitios estrechos.
	public BoxCollider2D Boxcol;

	//Cuando me matan reinicio la escena
	private Scene scene;

	//Para el audio
	public string jumpSound = "jump";
	private AudioManager audiomanager;

	//El start se reproduce sólo una vez cuando le damos al Play
	void Start()
	{
		//Aqui les estamos diciendo cuales exactamente son los componentes Rigidbody y BoxCollider con los que vamos a trabajar. Que serán los del personaje.
		rb = GetComponent<Rigidbody2D>();
		Boxcol = GetComponent<BoxCollider2D>();
		audiomanager = AudioManager.instance;
		audiomanager.StopSound("MenuMusic");
		audiomanager.PlaySound("Music");
		ScoreText.text = Score.ToString();
		scene = SceneManager.GetActiveScene();
		Hud.sprite = HealthSprites[Lifes - 1];
	}

	// El Update se reproduce una vez cada frame, por lo que realizará todo el código que esté aqui dentro una vez cada frame. 
	void Update()
	{
		//Recordemos que al poner aqui los nombres de los ámbitos inferiores, lo que hace el código es que salta a la parte inferior y continua leyendo donde corresponda. 
		if(Lifes != 0)
		{
			DirDetect();
			Movement();
		}
	}

	//En este ambito detectaremos la direccion a la que está mirando. Si X es positivo a la derecha, si X es negativo a la izquierda y si es 0 estará quieto
	private void DirDetect()
	{
		if(Input.GetAxis("Horizontal") > 0)
		{
			b_LookRight = true;
			//Recordemos que la rotación del personaje nos permite ponerle mirando en una dirección u otra.
			transform.rotation = Quaternion.Euler(0, 0, 0);
			Anim.SetBool("Move", true);
		}
		else if(Input.GetAxis("Horizontal") < 0)
		{
			b_LookRight = false;
			transform.rotation = Quaternion.Euler(0, 180, 0);
			Anim.SetBool("Move", true);
		}
		else
		{
			Anim.SetBool("Move", false);
		}
	}

	private void Movement()
	{
		//Intentamos mover a mi personaje con Translate, pero cuando se choca, funciona fatal.
		//transform.Translate(Vector2.right * Speed * Input.GetAxis("Horizontal") * Time.deltaTime);


		//En esta linea estamos dibujando imaginariamente el área de colision que hay entre el punto 1 y punto 2, y le estamos diciendo que reconozca cuando toca cualquier objeto
		//Que en unity tenga el Layer configurado como Suelo/Ground.


		b_IsGrounded = Physics2D.OverlapArea(Point1.transform.position, Point2.transform.position, Ground);
		b_WallDetect = Physics2D.OverlapArea(Point3.transform.position, Point4.transform.position, WallMask);

		//El IF nos permite decirle que realice lo que hay dentro del ámbito SOLO si se cumple la condición que hay entre paréntesis, en este caso que esté tocando el suelo.
		//Recordemos IsGrounded true será cuando toca el suelo. Si es false, estará en el aire. 
		if (b_IsGrounded == true)
		{
			Anim.SetBool("Jump", false);
			//SI pulsamos Up ocurrirá lo del ámbito. En este caso aplicaremos una fuerza hacia arriba igual al JumpSpeed. 
			if (Input.GetKeyDown(KeyCode.UpArrow))
			{
				rb.AddForce(Vector2.up * JumpSpeed);
				audiomanager.PlaySound(jumpSound);
			}

			//Esta parte es nueva y la he añadido yo, para que podais darle hacia abajo y que el personaje se agache. Hay que configurarlo también en el Unity para que las animaciones funcionen.
			//Como os voy a pasar el trabajo que yo realice, podreis ver como esta configurado más tarde. 
			if (Input.GetKeyDown(KeyCode.DownArrow))
			{
				Anim.SetBool("Crouch", true);
				Boxcol.enabled = false;
				//Esta velocidad se la he puesto para que al agacharse ande más despacito.
				CrouchSpeed = 0.2f;
			}
			else if (Input.GetKeyUp(KeyCode.DownArrow))
			{
				Anim.SetBool("Crouch", false);
				Boxcol.enabled = true;
				CrouchSpeed = 1f;
			}
			//Realizamos el movimiento usando las físicas y el Rigidbody
			rb.velocity = new Vector2(Speed * CrouchSpeed * Input.GetAxis("Horizontal"), rb.velocity.y);
		}
		else
		{
			Anim.SetBool("Jump", true);
			if (b_WallDetect == false)
			{
				rb.velocity = new Vector2(Speed * Input.GetAxis("Horizontal"), rb.velocity.y);
			}
		}

	}

	void HealthUpdate(int life)
	{
		if (Lifes == 0)
		{
			Boxcol.enabled = false;
			transform.gameObject.GetComponent<CircleCollider2D>().enabled = false;
			rb.velocity *= 0;
			rb.AddForce(Vector2.up * JumpSpeed * 1.2f);
			Anim.SetBool("Death", true);
			audiomanager.PlaySound("Hurt");
			Invoke("RestartScene", 2f);
		}
		else
			Hud.sprite = HealthSprites[life - 1];
	}

	void RestartScene()
	{
		SceneManager.LoadScene(scene.name);
	}

	private void OnTriggerEnter2D(Collider2D col)
	{
		//Cuando choco con un objeto, miro si su tag es Score. Ojo, el nombre bien escrito.
		if (col.tag == "Score")
		{
			//Sumar puntuacion
			Score++; //Score = Score + 1;
			Destroy(col.gameObject);
			ScoreText.text = Score.ToString();
			audiomanager.PlaySound("Star");
		}
		if (col.tag == "Health")
		{
			if (Lifes <= 3)
				Lifes++;
			HealthUpdate(Lifes);
			audiomanager.PlaySound("Carrot");
			Destroy(col.gameObject);
		}
		if (col.tag == "Cliff")
		{
			Lifes = 0;
			HealthUpdate(Lifes);
		}
		if (col.tag == "EDeath")
		{
			print("Entre");
			col.transform.parent.gameObject.GetComponent<Animator>().SetTrigger("Death");
			rb.velocity *= 0;
			rb.AddForce(Vector2.up * JumpSpeed/3);
			audiomanager.PlaySound("EnemyDestroy");
			Destroy(col.transform.parent.gameObject.transform.parent.gameObject, 0.3f);
		}
	}

	private void OnCollisionEnter2D(Collision2D col)
	{
		if (col.gameObject.tag == "Enemy")
		{
			Lifes--;
			Anim.SetTrigger("IsHit");
			audiomanager.PlaySound("Hurt");
			HealthUpdate(Lifes);
			Destroy(col.gameObject);
		}
	}
}
