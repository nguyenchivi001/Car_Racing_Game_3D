using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CarController : MonoBehaviour
{

  //CAR SETUP
      [Space(20)]
      [Header("CAR SETUP")]
      [Space(10)]

      //Tốc độ tối đa mà xe có thể đạt được tính bằng km/h.
      [Range(20, 200)]
      public int maxSpeed = 200; 

      //Tốc độ tối đa mà xe có thể đạt được khi lùi lại tính bằng km/h.
      [Range(10, 120)]
      public int maxReverseSpeed = 60;

      //Tốc độ tăng tốc của xe. 1 là chậm nhất và 10 là nhanh nhất.
      [Range(1, 50)]
      public int accelerationMultiplier = 2; 
      
      //Góc tối đa mà lốp có thể đạt được khi quay vô lăng.
      [Range(10, 45)]
      public int maxSteeringAngle = 27;
      
      //Tốc độ quay của vô lăng.
      [Range(0.1f, 1f)]
      public float steeringSpeed = 0.5f;
      
      //Độ mạnh của phanh bánh xe.
      [Range(100, 600)]
      public int brakeForce = 350;

      //Tốc độ giảm tốc của xe khi người dùng không sử dụng ga.
      [Range(1, 10)]
      public int decelerationMultiplier = 2;
      
      //Mức độ giảm ma sát của xe khi người dùng kéo phanh tay.
      [Range(1, 10)]
      public int handbrakeDriftMultiplier = 5;

      public float nitrusValue;

      public int originalAM;

      public bool isBoosting;

      public bool owned;

      //Vector chứa trọng tâm của xe ( x = 0 , z = 0 , 0 <= y <= 1.5 )
      public Vector3 bodyMassCenter; 

  //WHEELS
      [Header("WHEELS")]
      public GameObject frontLeftMesh;
      public WheelCollider frontLeftCollider;
      public GameObject frontRightMesh;
      public WheelCollider frontRightCollider;
      public GameObject rearLeftMesh;
      public WheelCollider rearLeftCollider;
      public GameObject rearRightMesh;
      public WheelCollider rearRightCollider;

    //SPEED TEXT (UI)
      [Space(20)]
      [Header("UI")]
      [Space(10)]
      public bool useUI = false;
      public Text carSpeedText;

    //SOUNDS
      [Space(20)]
      [Header("SOUNDS")]
      [Space(10)]
      public bool useSounds = false;
      public AudioSource carEngineSound;
      public AudioSource tireScreechSound;
      float initialCarEngineSoundPitch; // Used to store the initial pitch of the car engine sound.
    
    //INFO
      [Space(20)]
      [Header("INFO")]
      public int carPrice ;
      public string carName;
      public bool hasFinished = false;

    //CAR DATA

      [HideInInspector] public float carSpeed; //Lưu trữ tốc độ của xe
      [HideInInspector] public bool isDrifting; //Để biết xe có drift hay không
      [HideInInspector] public bool isTractionLocked; //Để biết bánh xe có bị khóa hay không.
      [HideInInspector] public float RPM;

    //PRIVATE VARIABLES
      Rigidbody carRigidbody;
      float steeringAxis; //Để biết xem vô lăng đã đạt đến giá trị tối đa hay chưa. Đi từ -1 đến 1.(đo góc lái)
      float throttleAxis; //Để biết xem ga đã đạt đến giá trị tối đa hay chưa. Đi từ -1 đến 1.
      float driftingAxis; //đi từ 0 tới 1
      float localVelocityZ; // lưu trữ vận tốc theo trục z 
      float localVelocityX; // lưu trữ vận tốc theo trục x
      bool deceleratingCar; // Để biết xe có đang lùi hay không 



      WheelFrictionCurve FLwheelFriction;
      float FLWextremumSlip;
      WheelFrictionCurve FRwheelFriction;
      float FRWextremumSlip;
      WheelFrictionCurve RLwheelFriction;
      float RLWextremumSlip;
      WheelFrictionCurve RRwheelFriction;
      float RRWextremumSlip;

    void Start()
    {
      carRigidbody = gameObject.GetComponent<Rigidbody>();
      carRigidbody.centerOfMass = bodyMassCenter;


      FLwheelFriction = new WheelFrictionCurve ();
        FLwheelFriction.extremumSlip = frontLeftCollider.sidewaysFriction.extremumSlip;
        FLWextremumSlip = frontLeftCollider.sidewaysFriction.extremumSlip;
        FLwheelFriction.extremumValue = frontLeftCollider.sidewaysFriction.extremumValue;
        FLwheelFriction.asymptoteSlip = frontLeftCollider.sidewaysFriction.asymptoteSlip;
        FLwheelFriction.asymptoteValue = frontLeftCollider.sidewaysFriction.asymptoteValue;
        FLwheelFriction.stiffness = frontLeftCollider.sidewaysFriction.stiffness;
      FRwheelFriction = new WheelFrictionCurve ();
        FRwheelFriction.extremumSlip = frontRightCollider.sidewaysFriction.extremumSlip;
        FRWextremumSlip = frontRightCollider.sidewaysFriction.extremumSlip;
        FRwheelFriction.extremumValue = frontRightCollider.sidewaysFriction.extremumValue;
        FRwheelFriction.asymptoteSlip = frontRightCollider.sidewaysFriction.asymptoteSlip;
        FRwheelFriction.asymptoteValue = frontRightCollider.sidewaysFriction.asymptoteValue;
        FRwheelFriction.stiffness = frontRightCollider.sidewaysFriction.stiffness;
      RLwheelFriction = new WheelFrictionCurve ();
        RLwheelFriction.extremumSlip = rearLeftCollider.sidewaysFriction.extremumSlip;
        RLWextremumSlip = rearLeftCollider.sidewaysFriction.extremumSlip;
        RLwheelFriction.extremumValue = rearLeftCollider.sidewaysFriction.extremumValue;
        RLwheelFriction.asymptoteSlip = rearLeftCollider.sidewaysFriction.asymptoteSlip;
        RLwheelFriction.asymptoteValue = rearLeftCollider.sidewaysFriction.asymptoteValue;
        RLwheelFriction.stiffness = rearLeftCollider.sidewaysFriction.stiffness;
      RRwheelFriction = new WheelFrictionCurve ();
        RRwheelFriction.extremumSlip = rearRightCollider.sidewaysFriction.extremumSlip;
        RRWextremumSlip = rearRightCollider.sidewaysFriction.extremumSlip;
        RRwheelFriction.extremumValue = rearRightCollider.sidewaysFriction.extremumValue;
        RRwheelFriction.asymptoteSlip = rearRightCollider.sidewaysFriction.asymptoteSlip;
        RRwheelFriction.asymptoteValue = rearRightCollider.sidewaysFriction.asymptoteValue;
        RRwheelFriction.stiffness = rearRightCollider.sidewaysFriction.stiffness;


      //init boost
      originalAM = accelerationMultiplier;
      nitrusValue = 2f;
      isBoosting = false;

      //init Sound
      if(carEngineSound != null)
      {
        initialCarEngineSoundPitch = carEngineSound.pitch;
      }

      //Sound
      if(useSounds)
      {
          InvokeRepeating("CarSounds", 0f, 0.1f);
      }
      else if(!useSounds)
      {
        if(carEngineSound != null)
        {
          carEngineSound.Stop();
        }
        if(tireScreechSound != null)
        {
          tireScreechSound.Stop();
        }
      }
      
      //UI
      if(useUI)
      {
          InvokeRepeating("CarSpeedUI", 0f, 0.1f);
      }
      else if(!useUI)
      {
        if(carSpeedText != null)
        {
          carSpeedText.text = "0";
        }
      }
    }

    void Update()
    {

      //CAR DATA

      //Tính toán vận tốc của xe
      RPM = frontLeftCollider.rpm;
      float ChuVi = 2 * Mathf.PI * frontLeftCollider.radius;
      carSpeed = (ChuVi * RPM * 60) / 1000; // km/h
      // Lưu trữ vận tốc của xe theo trục x. Được sử dụng để biết xem xe có đang sang trái hay phải không.
      localVelocityX = transform.InverseTransformDirection(carRigidbody.velocity).x;
      // Lưu trữ vận tốc của xe theo trục z. Được sử dụng để biết xem xe đang đi tiến hay lùi.
      localVelocityZ = transform.InverseTransformDirection(carRigidbody.velocity).z;

      //CAR PHYSICS

      //W (tăng ga), S (lùi),
      //A (rẽ trái), D (rẽ phải), Space bar (phanh-drifts), F (tăng tốc)

        if(Input.GetKey(KeyCode.W))
	      {
          CancelInvoke("DecelerateCar");
          deceleratingCar = false;
          GoForward();
        }

        if(Input.GetKey(KeyCode.S))
	      {
          CancelInvoke("DecelerateCar");
          deceleratingCar = false;
          GoReverse();
        }

        if(Input.GetKey(KeyCode.A))
	      {
          TurnLeft();
        }

        if(Input.GetKey(KeyCode.D))
	      {
          TurnRight();
        }

        if(Input.GetKey(KeyCode.Space))
	      {
          CancelInvoke("DecelerateCar");
          deceleratingCar = false;
          Handbrake();
        }
        
        if(Input.GetKeyUp(KeyCode.Space))
        {
          RecoverTraction();
        }

        if(Input.GetKeyUp(KeyCode.F))
        {
          RecoverNitrus();
        }
        if(Input.GetKey(KeyCode.F))
        {
            CancelInvoke("DecelerateCar");
            deceleratingCar = false;
            Boost();
        }

        if((!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W)))
	      {
          ThrottleOff();
        }

        if((!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W)) && !Input.GetKey(KeyCode.Space) && !deceleratingCar)
	      {
          InvokeRepeating("DecelerateCar", 0f, 0.1f);
          deceleratingCar = true;
        }

        if(!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && steeringAxis != 0f)
	      {
          ResetSteeringAngle();
        }

      SynchroWheelMeshes();
    }

    public void CarSpeedUI()
    {

      if(useUI)
      {
          try
          {
            float absoluteCarSpeed = Mathf.Abs(carSpeed);
            carSpeedText.text = Mathf.RoundToInt(absoluteCarSpeed).ToString();
          }
          catch(Exception ex)
          {
            Debug.LogWarning(ex);
          }
      }
    }

    public void CarSounds()
    {
      if(useSounds)
      {
        try
        {
          if(carEngineSound != null)
          {
            float engineSoundPitch = initialCarEngineSoundPitch + (Mathf.Abs(carRigidbody.velocity.magnitude) / 25f);
            carEngineSound.pitch = engineSoundPitch;
          }
          if((isDrifting) || (isTractionLocked && Mathf.Abs(carSpeed) > 12f))
          {
            if(!tireScreechSound.isPlaying)
            {
              tireScreechSound.Play();
            }
          }
          else if((!isDrifting) && (!isTractionLocked || Mathf.Abs(carSpeed) < 12f))
          {
            tireScreechSound.Stop();
          }
        }
        catch(Exception ex)
        {
          Debug.LogWarning(ex);
        }
      }
      else if(!useSounds)
      {
        if(carEngineSound != null && carEngineSound.isPlaying)
        {
          carEngineSound.Stop();
        }
        if(tireScreechSound != null && tireScreechSound.isPlaying)
        {
          tireScreechSound.Stop();
        }
      }
    }

    //Phương thức XOAY
    //
    //

    //Phương thức TurnLeft xoay bánh xe trước của xe sang trái. 
    //Tốc độ của chuyển động này sẽ phụ thuộc vào biến steeringSpeed.
    public void TurnLeft()
    {
      steeringAxis = steeringAxis - (Time.deltaTime * 10f * steeringSpeed);
      if(steeringAxis < -1f)
      {
        steeringAxis = -1f;
      }
      var steeringAngle = steeringAxis * maxSteeringAngle;
      frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
      frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
    }

    //Phương thức TurnRight xoay bánh xe trước của xe sang phải. 
    //Tốc độ của chuyển động này sẽ phụ thuộc vào biến steeringSpeed.
    public void TurnRight()
    {
      steeringAxis = steeringAxis + (Time.deltaTime * 10f * steeringSpeed);
      if(steeringAxis > 1f)
      {
        steeringAxis = 1f;
      }
      var steeringAngle = steeringAxis * maxSteeringAngle;
      frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
      frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
    }

    //Phương thức sau đây đưa bánh xe trước của xe về vị trí mặc định của chúng (quay = 0). 
    //Tốc độ của chuyển động này sẽ phụ thuộc vào biến steeringSpeed.
    public void ResetSteeringAngle()
    {
      if(steeringAxis < 0f)
      {
        steeringAxis = steeringAxis + (Time.deltaTime * 10f * steeringSpeed);
      }
      else if(steeringAxis > 0f)
      {
        steeringAxis = steeringAxis - (Time.deltaTime * 10f * steeringSpeed);
      }

      if(Mathf.Abs(frontLeftCollider.steerAngle) < 1f)
      {
        steeringAxis = 0f;
      }
      var steeringAngle = steeringAxis * maxSteeringAngle;
      frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
      frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
    }

    // Đồng bộ position and rotation của WheelColliders và WheelMeshes.
    void SynchroWheelMeshes()
    {
      try
      {
        Quaternion FLWRotation;
        Vector3 FLWPosition;                            
        frontLeftCollider.GetWorldPose(out FLWPosition, out FLWRotation);
        frontLeftMesh.transform.position = FLWPosition;
        frontLeftMesh.transform.rotation = FLWRotation;

        Quaternion FRWRotation;
        Vector3 FRWPosition;
        frontRightCollider.GetWorldPose(out FRWPosition, out FRWRotation);
        frontRightMesh.transform.position = FRWPosition;
        frontRightMesh.transform.rotation = FRWRotation;

        Quaternion RLWRotation;
        Vector3 RLWPosition;
        rearLeftCollider.GetWorldPose(out RLWPosition, out RLWRotation);
        rearLeftMesh.transform.position = RLWPosition;
        rearLeftMesh.transform.rotation = RLWRotation;

        Quaternion RRWRotation;
        Vector3 RRWPosition;
        rearRightCollider.GetWorldPose(out RRWPosition, out RRWRotation);
        rearRightMesh.transform.position = RRWPosition;
        rearRightMesh.transform.rotation = RRWRotation;
      }

      // nếu có ngoại lệ thì sẽ được in ra bằng logwarning
      catch(Exception ex)
      {
        Debug.LogWarning(ex);
      }
    }

  
    //Các phương thức ĐỘNG CƠ VÀ PHANH
    //
    //

    // Phương thức này áp dụng mô-men xoắn dương cho bánh xe để di chuyển về phía trước.
    public void GoForward()
    {
      if(Mathf.Abs(localVelocityX) > 2.5f)
      {
        isDrifting = true;
      }
      else
      {
        isDrifting = false;
      }
      // tính toán tốc độ tăng tốc của xe
      throttleAxis = throttleAxis + (Time.deltaTime * 3f);
      if(throttleAxis > 1f)
      {
        throttleAxis = 1f;
      }
      //nếu localVelocityZ < -1f tức xe đang đi lùi ( vì va chạm ,..) , gọi hàm brake() để phanh lại.
      if(localVelocityZ < -1f)
      {
        Brakes();
      }
      else
      {
        if(Mathf.RoundToInt(carSpeed) < maxSpeed)
        {
          //Áp dụng mô-men xoắn dương vào tất cả bánh xe để đi về phía trước nếu chưa đạt tới tốc độ tối đa.
          frontLeftCollider.brakeTorque = 0;
          frontLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
          frontRightCollider.brakeTorque = 0;
          frontRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
          rearLeftCollider.brakeTorque = 0;
          rearLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
          rearRightCollider.brakeTorque = 0;
          rearRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
        }
        else 
        {
          //Nếu xe đã đạt tốc độ tối đa thì set mô-men xoắn = 0.
    			frontLeftCollider.motorTorque = 0;
    			frontRightCollider.motorTorque = 0;
          rearLeftCollider.motorTorque = 0;
    			rearRightCollider.motorTorque = 0;
    		}
      }
    }

    //Phương thức này áp dụng mô-men xoắn âm cho bánh xe để di chuyển về phía sau
    public void GoReverse()
    {
      if(Mathf.Abs(localVelocityX) > 2.5f)
      {
        isDrifting = true;
      }
      else
      {
        isDrifting = false;
      }
      throttleAxis = throttleAxis - (Time.deltaTime * 3f);
      if(throttleAxis < -1f)
      {
        throttleAxis = -1f;
      }
      //nếu localVelocityZ > 1f tức xe đang đi tới , gọi hàm brake() để phanh lại.
      if(localVelocityZ > 1f)
      {
        Brakes();
      }
      else
      {
        if(Mathf.Abs(Mathf.RoundToInt(carSpeed)) < maxReverseSpeed)
        {
          //Áp dụng mô-men xoắn âm vào tất cả bánh xe để đi về phía sau nếu chưa đạt tới tốc độ lùi tối đa.
          frontLeftCollider.brakeTorque = 0;
          frontLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
          frontRightCollider.brakeTorque = 0;
          frontRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
          rearLeftCollider.brakeTorque = 0;
          rearLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
          rearRightCollider.brakeTorque = 0;
          rearRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
        }
        else 
        {
          // Nếu xe đã đạt tốc độ lùi tối đa thì set mô-men xoắn = 0.
    			frontLeftCollider.motorTorque = 0;
    			frontRightCollider.motorTorque = 0;
          rearLeftCollider.motorTorque = 0;
    			rearRightCollider.motorTorque = 0;
    		}
      }
    }

    //Hàm sau đặt mô-men xoắn động cơ thành 0 (trong trường hợp người dùng không nhấn W hoặc S).
    public void ThrottleOff()
    {
      frontLeftCollider.motorTorque = 0;
      frontRightCollider.motorTorque = 0;
      rearLeftCollider.motorTorque = 0;
      rearRightCollider.motorTorque = 0;
    }

    //Giảm tốc độ xe theo biến decelerationMultiplier ( trong trường hợp không nhấn W , S , Space )
    public void DecelerateCar()
    {
      if(Mathf.Abs(localVelocityX) > 2.5f)
      {
        isDrifting = true;
      }
      else
      {
        isDrifting = false;
      }
      // chuyển trạng thái tăng ga về 0 
      if(throttleAxis != 0f)
      {
        if(throttleAxis > 0f)
        {
          throttleAxis = throttleAxis - (Time.deltaTime * 10f);
        }
        else if(throttleAxis < 0f)
        {
            throttleAxis = throttleAxis + (Time.deltaTime * 10f);
        }
        if(Mathf.Abs(throttleAxis) < 0.15f)
        {
          throttleAxis = 0f;
        }
      }

      // giảm dần vận tốc của xe 
      carRigidbody.velocity = carRigidbody.velocity * (1f / (1f + (0.025f * decelerationMultiplier)));
      // đặt các mô-men xoắn về 0.
      frontLeftCollider.motorTorque = 0;
      frontRightCollider.motorTorque = 0;
      rearLeftCollider.motorTorque = 0;
      rearRightCollider.motorTorque = 0;
      //Nếu vận tốc của xe <0.25f = rất chậm , đặt vận tốc thành 0 và huỷ gọi phương thức này.
      if(carRigidbody.velocity.magnitude < 0.25f)
      {
        carRigidbody.velocity = Vector3.zero;
        CancelInvoke("DecelerateCar");
      }
    }

    // Hàm này áp dụng mô-men xoắn phanh cho bánh xe dựa trên lực phanh
    public void Brakes()
    {
      frontLeftCollider.brakeTorque = brakeForce;
      frontRightCollider.brakeTorque = brakeForce;
      rearLeftCollider.brakeTorque = brakeForce;
      rearRightCollider.brakeTorque = brakeForce;
    }

    // Hàm này được sử dụng để làm xe mất ma sát.Do đó xe sẽ bắt đầu trượt.
    public void Handbrake()
    {
      CancelInvoke("RecoverTraction");
      //Làm giảm ma sát chậm dần (giảm theo từng frame)
      //biến driftingAxis để xác định mức độ của drift ( 0 <= dA <= 1)
      driftingAxis = driftingAxis + (Time.deltaTime);
      float secureStartingPoint = driftingAxis * FLWextremumSlip * handbrakeDriftMultiplier;// tính toán độ trượt

      if(secureStartingPoint < FLWextremumSlip) // chưa đạt độ trượt tối đa
      {
        driftingAxis = FLWextremumSlip / (FLWextremumSlip * handbrakeDriftMultiplier); //secureStartingPoint = FLWextrenumSlip = độ trượt tối đa
      }
      if(driftingAxis > 1f)
      {
        driftingAxis = 1f;
      }

      //Nếu các lực được áp dụng vào Rigidbody theo trục 'x' lớn hơn 2.5f => xe đã mất độ trượt.
      if(Mathf.Abs(localVelocityX) > 2.5f)
      {
        isDrifting = true;
      }
      else
      {
        isDrifting = false;
      }
      
      //tăng độ trượt khi driftingAxis chưa đạt max
      if(driftingAxis < 1f)
      {
        FLwheelFriction.extremumSlip = FLWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
        frontLeftCollider.sidewaysFriction = FLwheelFriction;

        FRwheelFriction.extremumSlip = FRWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
        frontRightCollider.sidewaysFriction = FRwheelFriction;

        RLwheelFriction.extremumSlip = RLWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
        rearLeftCollider.sidewaysFriction = RLwheelFriction;

        RRwheelFriction.extremumSlip = RRWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
        rearRightCollider.sidewaysFriction = RRwheelFriction;
      }

      //đặt biến để biết rằng các bánh xe đang bị khoá 
      isTractionLocked = true;
    }


    // Hàm này được dùng để khôi phục ma sát của xe khi không nhấn space nữa
    public void RecoverTraction()
    {
      isTractionLocked = false;
      driftingAxis = driftingAxis - (Time.deltaTime / 1.5f);
      if(driftingAxis < 0f)
      {
        driftingAxis = 0f;
      }

      //đặt lại độ trượt của xe 
      if(FLwheelFriction.extremumSlip > FLWextremumSlip) // driftingAxis giảm theo từng frame , giảm dần độ trượt giúp drift mượt mà.
      {
        FLwheelFriction.extremumSlip = FLWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
        frontLeftCollider.sidewaysFriction = FLwheelFriction;

        FRwheelFriction.extremumSlip = FRWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
        frontRightCollider.sidewaysFriction = FRwheelFriction;

        RLwheelFriction.extremumSlip = RLWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
        rearLeftCollider.sidewaysFriction = RLwheelFriction;

        RRwheelFriction.extremumSlip = RRWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
        rearRightCollider.sidewaysFriction = RRwheelFriction;

        Invoke("RecoverTraction", Time.deltaTime);
      }
      else if (FLwheelFriction.extremumSlip < FLWextremumSlip)
      {
        FLwheelFriction.extremumSlip = FLWextremumSlip;
        frontLeftCollider.sidewaysFriction = FLwheelFriction;

        FRwheelFriction.extremumSlip = FRWextremumSlip;
        frontRightCollider.sidewaysFriction = FRwheelFriction;

        RLwheelFriction.extremumSlip = RLWextremumSlip;
        rearLeftCollider.sidewaysFriction = RLwheelFriction;

        RRwheelFriction.extremumSlip = RRWextremumSlip;
        rearRightCollider.sidewaysFriction = RRwheelFriction;

        driftingAxis = 0f;
      }
    }

    public void Boost()
    {
      CancelInvoke("RecoverNitrus");
      if(nitrusValue > 0)
      {
        isBoosting = true;
       nitrusValue -= Time.deltaTime * 1.5f;

        if(accelerationMultiplier == originalAM)
        {
          accelerationMultiplier *= 2;
        }
      }
      else
      {
        isBoosting = false;
        accelerationMultiplier = originalAM;
      }
    }
    public void RecoverNitrus()
    {
      isBoosting = false;
      if(nitrusValue < 6f)
      {
        nitrusValue += Time.deltaTime / 3;
      }
      if(nitrusValue > 6f)
      {
        nitrusValue = 6f;
      }
      Invoke("RecoverNitrus", Time.deltaTime);
      accelerationMultiplier = originalAM;
    }
}