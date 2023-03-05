using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using QAAPlatformer;
using QAAPlatformer.Character;
using QAAPlatformer.Damage;
using QAAPlatformer.Obstacles;
using static QAAPlatformer.GameManager;
using UnityEngine.SceneManagement;
using System.IO;



public class script : MonoBehaviour


{
    //Simple logger class that will fill up my log.txt
    class Logger
    {
        private string logFileName;

        public Logger(string fileName)
        {
            logFileName = fileName;
        }

        public void Log(string message)
        {
            using (StreamWriter writer = File.AppendText(logFileName))
            {
                writer.WriteLine(message);
            }
        }
    }
  
    Rigidbody2D m_rbody;
    protected CharacterDamageManager m_DamageManager;
    protected GameObject m_obj;

    Logger logger = new Logger("log.txt");






    [UnityTest]
    public IEnumerator DummyTest_LoadSceneAsync_MainSceneIsLoaded()

    //1. Start async loadin of "MainScene
    //2. Create while loop, when the scene is not loaded we wait, if MainScene doesnt load eventually we go to timeout
    //3. Check IsRunning Variable from GameManager class
    //4. Log result
    //Possible improvements: 1. Probably could check more than GameManager.IsRunning 
    {
        SceneManager.LoadSceneAsync("MainScene");
        bool result = false;

        while (SceneManager.GetSceneByName("MainScene").isLoaded == false)
        {
            yield return null;
        }


        result = GameManager.IsRunning;
        LogResult(result, "Loading test");
    }

  

    //1.
        [UnityTest]
    public IEnumerator DummyTest_MovementTest()

    //1. Get the instance of CharacterController2D
    //2. Use the public CalculateJump method I created to somehow fake input in tests, found this method on youtube so I hope its okay
    //3. Check whether Velocity has changed and x velocity remains same, since we only jump on y axis
    //4. Wait for the movement to finish
    //5. Try the same for moving left and right using CalculateMovement method.
    //Possible improvements: 1. I feel that modifying the update method with CalculateJump and CalculateMovement made the controls little weird, I have no idea how to do it otherwise or what caused it
    //                       2. Maybe a generalized method for movement that would take arguments for CalculateMovement and had logic to check would be nice
    //                       3. Instead of WaitForSeconds we could tie the continuation of test to maybe velocity reaching zero or smt like that.
    {
        bool result = false;
        m_rbody = getCurrentRigidBody();
        m_obj = GameObject.Find("Player");
        var controller = m_obj.GetComponent<CharacterController2D>();

        //Jump
        m_rbody.velocity = controller.CalculateJump(0, 0.01f);
        result = (m_rbody.velocity.y > 0);
        result &= (m_rbody.velocity.x == 0);
        yield return new WaitForSeconds(1f);

        //move Right
        Vector2 position = m_rbody.position;
        m_rbody.AddForce(controller.CalculateMovement(1f, 0.1f), ForceMode2D.Impulse);
        result &= (m_rbody.velocity.x > 0);
        yield return new WaitForSeconds(1f);
        Vector2 positionAfter = m_rbody.position;
        result &= (position.x < positionAfter.x);

        //move Left
        position = m_rbody.position;
        m_rbody.AddForce(controller.CalculateMovement(-1f, 0.1f), ForceMode2D.Impulse);
        result &= (m_rbody.velocity.x < 0);
        yield return new WaitForSeconds(1f);
        positionAfter = m_rbody.position;
        result &= (position.x > positionAfter.x);

        


        LogResult(result, "Movement test");


    }
    [UnityTest]
    public IEnumerator DummyTest_Portal_Trigger()

    {
        //1. Get Position of portal
        //2. Get rigidbody object
        //3. set Rigidbody position to position of portal
        //4. Check if IsRunning and Timer are false
        //Possible Improvements: I believe some further tests could try to reach portal when object is really fast and tests like that
        bool result = false;
        var port = GameObject.Find("PortalSprite").transform.position + new Vector3(0, 0);

        m_rbody = getCurrentRigidBody();

        
        m_rbody.position = port;
        yield return new WaitForSeconds(0.2f);

        result = (!GameManager.IsRunning);
        result &= (!GameManager.IsTimerRunning);

        LogResult(result, "Portal test");

    }

    [UnityTest]
    public IEnumerator DummyTest_SpikeTest()
    // For each spike:
    //1. Restart game
    //2. Use DummyTest_SpikeTests method that basicly just decides which spike we are testing and teleports our rigidbody to it
    //3. Check if player is dead and his HP, I think its good idea to check for both, since its usefull to know if our HP loss system works
    //4. I modifyied CharacterDamageManager class, since its method ReceiveDamage was adding instead of subtracting the hp and I wanted the tests to pass, hope thats ok :)
    //Possible Improvements: I dont like the code repetition, also I would like to check more triggers. Also the waits are silly
    {
        bool result = false;

        GameManager.RestartCurrentLevel();
        yield return new WaitForSeconds(1f);
        DummyTest_SpikeTests(1);
        yield return new WaitForSeconds(1f);
        result = (m_DamageManager.IsDead());
        result &= (m_DamageManager.PlayerHP() <= 0);



        GameManager.RestartCurrentLevel();
        yield return new WaitForSeconds(1f);
        DummyTest_SpikeTests(2);
        yield return new WaitForSeconds(1f);
        result &= (m_DamageManager.IsDead());
        result &= (m_DamageManager.PlayerHP() <= 0);

        GameManager.RestartCurrentLevel();
        yield return new WaitForSeconds(1f);
        DummyTest_SpikeTests(3);
        yield return new WaitForSeconds(1f);
        result &= (m_DamageManager.IsDead());
        result &= (m_DamageManager.PlayerHP() <= 0);

        GameManager.RestartCurrentLevel();
        yield return new WaitForSeconds(1f);
        DummyTest_SpikeTests(4);
        yield return new WaitForSeconds(1f);
        result &= (m_DamageManager.IsDead());
        result &= (m_DamageManager.PlayerHP() <= 0);

        LogResult(result, "Spike test");
    }

    //Method for teleporting on spikes
    public void DummyTest_SpikeTests(int spikeNumber)
    {

        var spike = GameObject.Find("Spike").transform.position + new Vector3(0, 0);
        m_rbody = getCurrentRigidBody();
        m_DamageManager = getCurrentDamageManager();
        // m_DamageManager = m_obj.GetComponent<CharacterDamageManager>();


        switch (spikeNumber)
        {
            case 1:
                spike = GameObject.Find("Spike").transform.position + new Vector3(0, 0);
                break;
            case 2:
                spike = GameObject.Find("Spike (1)").transform.position + new Vector3(0, 0);
                break;
            case 3:
                spike = GameObject.Find("Spike (2)").transform.position + new Vector3(0, 0);
                break;
            case 4:
                spike = GameObject.Find("Spike (3)").transform.position + new Vector3(0, 0);
                break;
            default:

                break;

        }
        m_rbody.position = spike;



    }

    //Method for teleporting on spikes
    public void LogResult(bool result, string test)
    {
        var message = result ? "PASSED" : "FAILED";
        logger.Log(test + ": " +message);
    }

    //Get player rigidbody
    public Rigidbody2D getCurrentRigidBody()
    {
        m_obj = GameObject.Find("Player");
       
        return m_obj.GetComponent<Rigidbody2D>();
    }

    //Get player damage manager
    public CharacterDamageManager getCurrentDamageManager()
    {
        m_obj = GameObject.Find("Player");
        
        return m_obj.GetComponent<CharacterDamageManager>();
    }

}