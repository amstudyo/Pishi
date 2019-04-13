using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum CAction
{
	None,
	Jump,
	Run,
	Sit
}

[System.Serializable]
public class CCameraProp
{
	public float X;
	public float Y;
    public float Z;
	public Quaternion Rot;
}

public class CCat : MonoBehaviour {

	public float Speed = 10f;
	public AudioClip []m_SE;
	public GameObject []Things;
    public GameObject CarPrefab;
	public AudioClip[] m_Musics;
	public GameObject DeathPage;
    public GameObject BringFood;
    public CCameraProp m_StreetCamProp;
    public CCameraProp m_DesertCamProp;
    public CCameraProp m_ParkCamProp;
    public CCameraProp m_AparCamProp;
    public CCameraProp m_MozeCamProp;


    Rigidbody rb;
	AudioSource m_Audio;
	Animator m_Anim;
	Transform ThrowPos;
	AudioSource m_HeartBeat;

	CAction m_Action = CAction.None;
	float m_ActionTimer = 0f;
    float m_InputTimer = 0f;
	bool m_DidRight = false;
	float m_RunTimer;
	bool m_Alive = true;
    bool forwardJump = false;
    bool throwSomething = false;

    public enum State { House, Street, Desert, Park , Apartment, Moze};
    State m_State = State.House;

	// Touch Stuff
	Vector2 m_TouchStart;
	bool m_Touched = false;
	int m_SwipeDir = 0;

	float TW;
	float TH;

	// Camera Stuff
	Transform m_Camera;
	Vector3 CameraOffset;
	Quaternion CameraRotation;

	void Awake()
	{
		rb = GetComponent<Rigidbody> ();
		m_Audio = GetComponent<AudioSource> ();
		m_Anim = GetComponentInChildren<Animator> ();
		ThrowPos = transform.FindChild ("ThrowPosition");
		m_Camera = transform.FindChild ("Main Camera");
		CameraOffset = m_Camera.localPosition;
		CameraRotation = m_Camera.localRotation;
		m_HeartBeat = transform.FindChild ("HeartBeat").GetComponent<AudioSource> ();

		TW = Screen.width / 200;
		TH = Screen.height / 200;

        RandomizeStreet("Street1");
        RandomizeStreet("Street2");
        RandomizeStreet("Street3");
        RandomizeStreet("Street4");
    }

    void RandomizeStreet(string ParentName)
    {
        Transform parent = GameObject.Find(ParentName).transform;
        List<Transform> lines1 = new List<Transform>();
        List<Transform> lines2 = new List<Transform>();
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.tag == "Street_Line1")
                lines1.Add(child);
            else
                lines2.Add(child);
        }
        lines1[Random.Range(0, lines1.Count)].GetComponent<BoxCollider>().enabled = true;
        lines2[Random.Range(0, lines1.Count)].GetComponent<BoxCollider>().enabled = true;
    }


	void Start()
	{
		Camera.main.GetComponent<AudioSource> ().clip = m_Musics [CContext.Context.GameMusic];
		Camera.main.GetComponent<AudioSource> ().Play ();
	}

	IEnumerator DieIE(float f)
	{
		yield return new WaitForSeconds (f);
		Application.LoadLevel ("MainMenu");
	}

	void Update()
	{
		if (!m_Alive)
			return;

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

		// Handle Camera
		Vector3 temp1 = Vector3.zero;
		m_Camera.localPosition = Vector3.SmoothDamp (m_Camera.localPosition, CameraOffset, ref temp1, 0.07f);
		m_Camera.localRotation = Quaternion.Lerp (m_Camera.localRotation, CameraRotation, 2 * Time.deltaTime);

		bool grounded = Physics.Raycast (transform.position+Vector3.up, Vector3.down, (m_State == State.Park ? 3.5f : 2f));
        if (rb.velocity.y > 20)
            grounded = false;
		if (grounded) {
			Vector3 vel = rb.velocity;
			vel.z = -Speed;

			if (m_RunTimer > 0f)
			{
                //vel.z *= 2f;
                vel.z = -50f;
				m_Anim.speed = 1.7f;
				m_RunTimer -= Time.deltaTime;
				if (m_RunTimer <= 0f)
					m_Anim.speed = 1f;
			}
            m_Anim.speed = 1f;
            rb.velocity = vel;
		}

		// Touch Handle
		Vector2 MousePos = Input.mousePosition;
		if (Input.GetMouseButtonDown (0)) 
		{
			m_Touched = true;
			m_TouchStart = MousePos;
			m_SwipeDir = 0;
		}
		if (Input.GetMouseButtonUp (0)) 
			m_Touched = false;
		//if (Input.GetMouseButtonUp (0)) 
		{
			if (m_Touched)
			{

				Vector2 diff = MousePos - m_TouchStart;
				if (Mathf.Abs (diff.x) < 10 * TW)
				{
					if (diff.y > 40 * TH)
						m_SwipeDir = 1;
					else if (diff.y < -40 * TH)
						m_SwipeDir = 3;
				}
				if (Mathf.Abs (diff.y) < 15 * TH)
				{

					if (diff.x > 25 * TW)
						m_SwipeDir = 4;
					else if (diff.x < -25 * TW)
						m_SwipeDir = 2;
				}
				if (diff.magnitude > 300)
					m_Touched = false;
			}
		}

		if (m_ActionTimer > 0f) 
		{
			m_ActionTimer -= Time.deltaTime;
			if (m_ActionTimer <= 0)
			{
				m_Camera.GetComponent<AudioSource>().volume = 0.4f;
				m_HeartBeat.Stop ();
				if (m_DidRight)
				{
					m_ActionTimer = 0f;
                    if (m_Action == CAction.Jump)
                    {
                        if (throwSomething)
                        {
                            ThrowSomething(0);
                            throwSomething = false;
                        }
						m_Anim.SetTrigger("Jump");
						Vector3 vell = rb.velocity;
						vell.z *= 1.2f;
						vell.y = 50;
                        if (m_State == State.Desert && forwardJump)
                        {
                            m_Anim.speed = 2f;
                            vell.z = -60f;
                            vell.y = 30f;
                            forwardJump = false;
                        }
                        if (m_State == State.Moze && forwardJump)
                        {
                            m_Anim.speed = 1.4f;
                            vell.z = -60f;
                            vell.y = 40f;
                            forwardJump = false;
                        }
                        rb.velocity = vell;
					}
					else if (m_Action == CAction.Sit)
					{
                        if (throwSomething)
                        {
                            ThrowSomething(1);
                            throwSomething = false;
                        }
                        else if (m_State == State.Street)
                            SendACar(0);
                        m_Anim.SetTrigger("Sit");
					}
					else if (m_Action == CAction.Run)
					{
                        if (throwSomething)
                        {
                            ThrowSomething(2);
                            throwSomething = false;
                        }
                        else if (m_State == State.Street)
                            SendACar(1);
                        m_RunTimer = 0.8f;
					}
					Debug.Log ("Good Job");
				}
				else
				{
					Camera.main.GetComponent<AudioSource> ().Stop ();
					m_Alive = false;
					DeathPage.SetActive(true);
					DeathPage.GetComponent<AudioSource>().Play();
					StartCoroutine(DieIE(2f));
                    CContext.Context.LoseCount++;
				}
				m_DidRight = false;
				m_Action = CAction.None;
			}
            if (m_InputTimer > 0f)
            {
                if (m_Action == CAction.Jump && m_SwipeDir == 1)
                    m_DidRight = true;
                if (m_Action == CAction.Sit && m_SwipeDir == 3)
                    m_DidRight = true;
                if (m_Action == CAction.Run && m_SwipeDir == 4)
                    m_DidRight = true;
                m_InputTimer -= Time.deltaTime;
            }

		}

		m_SwipeDir = 0;
	}

	void ThrowSomething(int act)
	{
        int rnd = Random.Range(0, Things.Length);
        GameObject thing = (GameObject)Instantiate (Things [rnd], ThrowPos.position + -Vector3.forward*30, 
		                                            Quaternion.identity);
        if (rnd == 0)
            thing.GetComponent<Rigidbody>().angularVelocity = Vector3.right * -20f;
		//thing.transform.SetParent (GameObject.Find ("Cat").transform); // TODO
		if (act == 0) {
			thing.transform.position += Vector3.down * 27f + Vector3.left * 18;
			thing.GetComponent<Rigidbody> ().velocity = new Vector3 (270, 20, 0);
		} else if (act == 1) {
			thing.transform.position += Vector3.down * 22f;
			thing.GetComponent<Rigidbody> ().velocity = new Vector3 (250, 18, 0);
		}
		else if (act == 2) {
			thing.transform.position -= Vector3.forward * 15f;
			thing.GetComponent<Rigidbody> ().velocity = new Vector3 (150, 20, 0);
		}
		Destroy (thing, 7f);
	}
    void SendACar(int act)
    {
        GameObject car = (GameObject)Instantiate(CarPrefab, transform.position + Vector3.right * 200f + Vector3.up * 5f,
                                                    Quaternion.identity);
        if (act == 0)
        {
            car.transform.position += -Vector3.forward * 17f;
            car.GetComponent<Rigidbody>().velocity = new Vector3(-300, 0, 0);
        }
        else if (act == 1)
        {
            car.transform.position += -Vector3.forward * 10f;
            car.GetComponent<Rigidbody>().velocity = new Vector3(-200, 0, 0);
        }
        Destroy(car, 6f);
    }

	void OnTriggerEnter(Collider Col)
	{
		if (!m_Alive)
			return;
		if (m_Action != CAction.None)
			return;
        if (Col.transform.tag == "House_Trigger")
        {
            Random.seed = (int)System.DateTime.Now.Ticks;
            int rnd = Random.Range(0, 3);
            m_ActionTimer = 2f;
            m_InputTimer = 2f;
            throwSomething = true;

            if (rnd == 0) {
                m_Action = CAction.Jump;
                m_Audio.PlayOneShot(m_SE[0]);
            } else if (rnd == 1) {
                m_Action = CAction.Sit;
                m_Audio.PlayOneShot(m_SE[1]);
            } else if (rnd == 2) {
                m_Action = CAction.Run;
                m_Audio.PlayOneShot(m_SE[2]);
            }
            m_Camera.GetComponent<AudioSource>().volume = 0.1f;
            m_HeartBeat.Play();
            Debug.Log(rnd);
        }
        else if (Col.transform.tag == "Street_Line1" || Col.transform.tag == "Street_Line2")
        {
            int rnd = Random.Range(1, 3);
            m_ActionTimer = 2f;
            m_InputTimer = 2f;

            if (rnd == 1)
            {
                m_Action = CAction.Sit;
                m_Audio.PlayOneShot(m_SE[1]);
            }
            else if (rnd == 2)
            {
                m_Action = CAction.Run;
                m_Audio.PlayOneShot(m_SE[2]);
            }
            m_Camera.GetComponent<AudioSource>().volume = 0.1f;
            m_HeartBeat.Play();
            Debug.Log(rnd);
        }
        else if (Col.transform.tag == "Desert_Trigger_Jump")
        {
            m_ActionTimer = 3f;
            m_InputTimer = 1.5f;
            m_Action = CAction.Jump;
            m_Audio.PlayOneShot(m_SE[0]);
            m_Camera.GetComponent<AudioSource>().volume = 0.1f;
            m_HeartBeat.Play();
            if (Col.transform.name == "forawrd")
                forwardJump = true;
        }
        else if (Col.transform.tag == "Desert_Trigger_JumpFast")
        {
            m_ActionTimer = 0.5f;
            m_InputTimer = 0.5f;
            m_Action = CAction.Jump;
            m_Audio.PlayOneShot(m_SE[0]);
            m_Camera.GetComponent<AudioSource>().volume = 0.1f;
            m_HeartBeat.Play();
        }
        else if (Col.transform.tag == "Desert_Trigger_Sit")
        {
            m_ActionTimer = 3f;
            m_InputTimer = 1.5f;
            m_Action = CAction.Sit;
            m_Audio.PlayOneShot(m_SE[1]);
            m_Camera.GetComponent<AudioSource>().volume = 0.1f;
            m_HeartBeat.Play();
        }
        else if (Col.transform.tag == "Park_Trigger_Sit")
        {
            m_ActionTimer = 3f;
            m_InputTimer = 1.5f;
            m_Action = CAction.Sit;
            m_Audio.PlayOneShot(m_SE[1]);
            m_Camera.GetComponent<AudioSource>().volume = 0.1f;
            m_HeartBeat.Play();
        }
        else if (Col.transform.tag == "Park_Trigger_Jump")
        {
            m_ActionTimer = 3f;
            m_InputTimer = 1.5f;
            m_Action = CAction.Jump;
            m_Audio.PlayOneShot(m_SE[0]);
            m_Camera.GetComponent<AudioSource>().volume = 0.1f;
            m_HeartBeat.Play();
        }
        else if (Col.transform.tag == "Apartment_Trigger_Jump")
        {
            m_ActionTimer = 1f;
            m_InputTimer = 1f;
            m_Action = CAction.Jump;
            m_Audio.PlayOneShot(m_SE[0]);
            m_Camera.GetComponent<AudioSource>().volume = 0.1f;
            m_HeartBeat.Play();
        }
        else if (Col.transform.tag == "Moze_Trigger_Jump")
        {
            m_ActionTimer = 3f;
            m_InputTimer = 1.5f;
            m_Action = CAction.Jump;
            m_Audio.PlayOneShot(m_SE[0]);
            m_Camera.GetComponent<AudioSource>().volume = 0.1f;
            m_HeartBeat.Play();
            forwardJump = true;
        }
        else if (Col.transform.tag == "ChangeCamera_Street") {
            Camera.main.transform.FindChild("Steam1").GetComponent<ParticleSystem>().startColor = new Color(0.8f, 0.8f, 0.8f);
            Camera.main.transform.FindChild("Steam2").GetComponent<ParticleSystem>().startColor = new Color(0.8f, 0.8f, 0.8f);
            m_State = State.Street;
            CameraOffset = new Vector3(m_StreetCamProp.X, m_StreetCamProp.Y , CameraOffset.z);
			CameraRotation = Quaternion.Euler(m_StreetCamProp.Rot.x, m_StreetCamProp.Rot.y, m_StreetCamProp.Rot.z);
		}
        else if (Col.transform.tag == "ChangeCamera_Desert")
        {
            Camera.main.transform.FindChild("Steam1").GetComponent<ParticleSystem>().startColor = new Color(139f/255f, 102f/255f, 65f/255f);
            Camera.main.transform.FindChild("Steam2").GetComponent<ParticleSystem>().startColor = new Color(139f / 255f, 102f / 255f, 65f / 255f);
            Speed = 30f;
            m_State = State.Desert;
            CameraOffset = new Vector3(m_DesertCamProp.X, m_DesertCamProp.Y, m_DesertCamProp.Z);
            CameraRotation = Quaternion.Euler(m_DesertCamProp.Rot.x, m_DesertCamProp.Rot.y, m_DesertCamProp.Rot.z);
        }
        else if (Col.transform.tag == "ChangeCamera_Park")
        {
            Camera.main.transform.FindChild("Steam1").GetComponent<ParticleSystem>().startColor = new Color(0.8f, 0.8f, 0.8f);
            Camera.main.transform.FindChild("Steam2").GetComponent<ParticleSystem>().startColor = new Color(0.8f, 0.8f, 0.8f);
            Speed = 35f;
            m_State = State.Park;
            CameraOffset = new Vector3(m_ParkCamProp.X, m_ParkCamProp.Y, CameraOffset.z);
            CameraRotation = Quaternion.Euler(m_ParkCamProp.Rot.x, m_ParkCamProp.Rot.y, m_ParkCamProp.Rot.z);
        }
        else if (Col.transform.tag == "ChangeCamera_Apartment")
        {
            Camera.main.transform.FindChild("Steam1").GetComponent<ParticleSystem>().startColor = new Color(0.8f, 0.8f, 0.8f);
            Camera.main.transform.FindChild("Steam2").GetComponent<ParticleSystem>().startColor = new Color(0.8f, 0.8f, 0.8f);
            Speed = 30f;
            m_State = State.Apartment;
            CameraOffset = new Vector3(m_AparCamProp.X, m_AparCamProp.Y, CameraOffset.z);
            CameraRotation = Quaternion.Euler(m_AparCamProp.Rot.x, m_AparCamProp.Rot.y, m_AparCamProp.Rot.z);
        }
        else if (Col.transform.tag == "ChangeCamera_Moze")
        {
            Camera.main.transform.FindChild("Steam1").GetComponent<ParticleSystem>().startColor = new Color(0.8f, 0.8f, 0.8f);
            Camera.main.transform.FindChild("Steam2").GetComponent<ParticleSystem>().startColor = new Color(0.8f, 0.8f, 0.8f);
            Speed = 30f;
            m_State = State.Moze;
            CameraOffset = new Vector3(m_MozeCamProp.X, m_MozeCamProp.Y, CameraOffset.z);
            CameraRotation = Quaternion.Euler(m_MozeCamProp.Rot.x, m_MozeCamProp.Rot.y, m_MozeCamProp.Rot.z);
        }
        else if (Col.transform.tag == "The_End")
        {
            Camera.main.GetComponent<AudioSource>().Stop();
            m_Alive = false;
            BringFood.SetActive(true);
            StartCoroutine(DieIE(4f));
            CContext.Context.FinishedCount++;
        }
    }
}
