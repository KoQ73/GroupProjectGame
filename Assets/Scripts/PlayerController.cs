using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using static UnityEngine.UI.CanvasScaler;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float movementSpeed = 2f;
    [SerializeField] int moveDistance = 2;
    public int playerHealth;
    public int shield = 0;
    public int maxPlayerHealth;
    public Slider hpSlider;
    public Slider shieldSlider;

    public TextMeshProUGUI hpText;
    public TextMeshProUGUI shieldText;

    private int currentHealth;

    [SerializeField] Material movableTile;
    [SerializeField] Material inactiveTile;
    [SerializeField] Material attackableTile;

    private AudioManager audioManager;
    private GameObject oldTile;
    private GameObject currentTile;
    private Vector2Int currentLocation;
    private Vector2Int selectedLocation;
    private bool isMoving = false;
    private bool isAttacking = false;
    private bool move = false;
    private List<Tile> pathList = new List<Tile>();
    private List<Tile> inRangeTiles = new List<Tile>();

    public Transform selectedUnit;
    List<Vector2Int> movableCords;
    List<Vector2Int> attackableCords;

    GridManager gridManager;
    CardManager cardManager;
    PathFinderA pathFinder;
    private RangeFinder rangeFinder;

    // Animator
    Animator animator;

    //public GameObject RewardUI;  // Reference to the Reward UI GameObject
    private RandomCardReward RandomCardReward;

    // Start is called before the first frame update
    private void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        cardManager = FindObjectOfType<CardManager>();
        pathFinder = new PathFinderA();
        rangeFinder = new RangeFinder();

        movableCords = new List<Vector2Int>();
        attackableCords = new List<Vector2Int>();

        selectedUnit = GameObject.FindGameObjectWithTag("Player").transform;
        currentLocation = new Vector2Int();
        selectedLocation = new Vector2Int();

        maxPlayerHealth = 20;
        playerHealth = maxPlayerHealth;
        hpSlider.maxValue = maxPlayerHealth;
        hpSlider.value = playerHealth;
        hpText.text = playerHealth.ToString();
        shieldSlider.value = shield;
        shieldText.text = shield.ToString();

        audioManager = FindObjectOfType<AudioManager>();

        animator = selectedUnit.GetComponent<Animator>();   // Animator for player

        RandomCardReward = FindObjectOfType<RandomCardReward>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (isMoving)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                bool hasHit = Physics.Raycast(ray, out hit);

                if (hasHit)
                {
                    if (hit.transform.tag == "Tile")
                    {
                        selectedLocation = hit.transform.GetComponent<Labeller>().cords;
                        currentTile = GameObject.Find("(" + selectedLocation.x.ToString() + ", " + selectedLocation.y.ToString() + ")");
                        foreach(Transform child in currentTile.transform) {
                            child.GetComponentInChildren<MeshRenderer>().material.color = Color.green;
                        }
                        selectedUnit.transform.position = new Vector3(selectedLocation.x, selectedUnit.position.y, selectedLocation.y);

                        pathList = pathFinder.findPath(gridManager.GetTile(currentLocation), gridManager.GetTile(selectedLocation));

                    }
                    if(oldTile != null && oldTile != currentTile){
                        foreach(Transform child in oldTile.transform) {
                            child.GetComponentInChildren<MeshRenderer>().material.color = Color.white;
                        }
                        oldTile.GetComponentInChildren<MeshRenderer>().material=movableTile;
                        //UnityEngine.Debug.Log("current: "+ oldTile);
                    }
                    oldTile = currentTile;
                }
            }
        }

        if (move)
        {
            MoveAlongPath();
        }

        if (isAttacking)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                bool hasHit = Physics.Raycast(ray, out hit);

                if (hasHit)
                {
                    if (hit.transform.tag == "Tile")
                    {
                        selectedLocation = hit.transform.GetComponent<Labeller>().cords;
                        currentTile = GameObject.Find("(" + selectedLocation.x.ToString() + ", " + selectedLocation.y.ToString() + ")");
                        foreach(Transform child in currentTile.transform) {
                            child.GetComponentInChildren<MeshRenderer>().material.color = Color.red;
                        }

                    }
                    if(oldTile != null && oldTile != currentTile){
                        foreach(Transform child in oldTile.transform) {
                            child.GetComponentInChildren<MeshRenderer>().material.color = Color.white;
                        }
                        if (oldTile == GameObject.Find("("+ selectedUnit.position.x.ToString()+", "+selectedUnit.position.z.ToString()+")")){
                        oldTile.GetComponentInChildren<MeshRenderer>().material=inactiveTile;
                        }
                        else{
                        oldTile.GetComponentInChildren<MeshRenderer>().material=attackableTile;
                        }
                    }
                    oldTile = currentTile;
                }
            }
        }
        hpSlider.value = playerHealth;
        hpText.text = playerHealth.ToString();
        int temp = shield;
        temp = Mathf.Clamp(shield,0,12);
        shieldSlider.value = shield;
        shieldText.text = temp.ToString();

        // if (Input.GetKeyDown(KeyCode.Space)) {
        //     HealCard(2);
        // }
        // if (Input.GetKeyDown(KeyCode.Return))
        // {
        //     CancelHealCard();
        // }
        // if (Input.GetKeyDown(KeyCode.LeftShift))
        // {
        //     SlashAttackCard();
        // }
        // if (Input.GetKeyDown(KeyCode.Comma))
        // {
        //     CancelSlashAttackCard();
        // }
        // if (Input.GetKeyDown(KeyCode.LeftControl))
        // {
        //     ConfirmSlashAttackCard(4);
        // }
        // if (Input.GetKeyDown(KeyCode.Tab))
        // {
        //     ExecuteCard();
        // }
        // if (Input.GetKeyDown(KeyCode.Equals))
        // {
        //     ConfirmExecuteCard(1);
        // }
        // if (Input.GetKeyDown(KeyCode.RightShift))
        // {
        //     CancelExecuteCard();
        // }
    }

    private void MoveAlongPath()
    {

        if (pathList.Count > 0)
        {
            animator.SetBool("isWalking", true);    // Change animation
            selectedUnit.LookAt(pathList[0].transform.position);    // Face direction

            var step = movementSpeed * Time.deltaTime;
            selectedUnit.position = Vector3.MoveTowards(selectedUnit.position, new Vector3(pathList[0].transform.position.x, selectedUnit.position.y, pathList[0].transform.position.z), step);

            if (Vector3.Distance(selectedUnit.position, new Vector3(pathList[0].transform.position.x, selectedUnit.position.y, pathList[0].transform.position.z)) < 0.0001f)
            {
                PositionPlayerOnTile();

                pathList.RemoveAt(0);

                if (pathList.Count <= 0)
                {
                    animator.SetBool("isWalking", false);   // Revert back to idle animation
                    move = false;
                    cardManager.BackToCards();
                }
                else
                {
                    selectedUnit.LookAt(new Vector3(pathList[0].transform.position.x, selectedUnit.position.y, pathList[0].transform.position.z));
                }
            }
        }
        else
        {
            animator.SetBool("isWalking", false);   // Revert back to idle animation
            move = false;
            cardManager.BackToCards();
        }


    }

    private void PositionPlayerOnTile()
    {
        selectedUnit.position = new Vector3(pathList[0].transform.position.x, selectedUnit.position.y, pathList[0].transform.position.z);
    }

    public void MovementCard()
    {
        //PopulateMovableArea();

        currentLocation = new Vector2Int((int)selectedUnit.position.x, (int)selectedUnit.position.z);

        GetInRangeTiles();
        EnableMovableTiles();

        isMoving = true;
    }

    public void CancelMovementCard()
    {
        DisableMovableTiles();
        selectedUnit.transform.position = new Vector3(currentLocation.x, selectedUnit.position.y, currentLocation.y);
        isMoving = false;
    }

    public void ConfirmMovementCard()
    {
        DisableMovableTiles();

        selectedUnit.transform.position = new Vector3(currentLocation.x, selectedUnit.position.y, currentLocation.y);

        move = true;
        isMoving = false;

    }

    public void AttackCard()
    {
        PopulateAttackArea("Basic");
        EnableAttackableTiles();

        currentLocation.x = (int)selectedUnit.position.x;
        currentLocation.y = (int)selectedUnit.position.z;
        isAttacking = true;
    }

    public void ConfirmAttackCard(int dmg)
    {
        // Animate it
        // face the direction of the enemy to attack
        selectedUnit.LookAt(new Vector3(selectedLocation.x, selectedUnit.position.y, selectedLocation.y));
        StartCoroutine(WaitAndAnimate(1.0f, "isBasicAttack"));
        StartCoroutine(audioManager.WaitAndPlaySFX(0.7f,"attack"));

        DisableAttackableTiles();
        DealAttack(dmg);

        isAttacking = false;
    }

    public void CancelAttackCard()
    {
        DisableAttackableTiles();

        isAttacking = false;
    }

    public void DealAttack(int dmg){
        // Check which unit is selected
        List<Unit> units = FindObjectOfType<UnitController>().Units;
        List<Unit> obstacles = FindObjectOfType<UnitController>().Obstacles;


        // Check which units to destroy
        List<Unit> toDestroy = new List<Unit>();

        for (int i = 0; i < units.Count; i++)
        {
            Unit u = units[i];
            if (u.cords == selectedLocation)
            {
                u.health -= dmg;
                // if health after is zero or less than zero, add it to toDestroy List
                if (u.health <= 0)
                {
                    toDestroy.Add(u);
                }
                else
                {
                    UnitHealthBar unitHealthBar = u.unitGameObject.GetComponentInChildren<UnitHealthBar>();
                    if (unitHealthBar != null)
                    {
                        unitHealthBar.UpdateHealthbar((float)u.health, (float)u.maxHealth);
                    }
                }
                break;
            }
        }

        foreach(Unit u  in toDestroy)
        {
            // Destroy the gameObject
            Destroy(u.unitGameObject, 1);
            // Release the tile
            gridManager.ReleaseTile(u.cords);
            // Remove in array
            units.Remove(u);
        }

        for (int i = 0; i < obstacles.Count; i++)
        {
            Unit block = obstacles[i];
            if (block.cords == selectedLocation)
            {
                block.health -= dmg;
                // if health after is zero or less than zero, add it to toDestroy List
                if (block.health <= 0)
                {
                    toDestroy.Add(block);
                }
                else
                {
                    // UnitHealthBar unitHealthBar = u.unitGameObject.GetComponentInChildren<UnitHealthBar>();
                    // if (unitHealthBar != null)
                    // {
                    //     unitHealthBar.UpdateHealthbar((float)u.health, (float)u.maxHealth);
                    // }
                }
                break;
            }
        }

        foreach(Unit block  in toDestroy)
        {
            // Destroy the gameObject
            Destroy(block.unitGameObject, 1);
            // Release the tile
            gridManager.ReleaseTile(block.cords);
            // Remove in array
            obstacles.Remove(block);
        }

        // Find UnitController and make isDefeated true
        //UnityEngine.Debug.Log(units.Count);
        checkLevelOver();
    }

    public void SlashAttackCard()
    {
        PopulateAttackArea("Slash");
        EnableAttackableTiles();
    }

    public void ConfirmSlashAttackCard(int dmg)
    {
        StartCoroutine(WaitAndAnimate(1.0f, "isSlashAttack"));
        StartCoroutine(audioManager.WaitAndPlaySFX(1f,"attack"));        
        DisableAttackableTiles();
        DealSlashAttack(dmg);
    }

    public void CancelSlashAttackCard()
    {
        DisableAttackableTiles();
    }

    private Unit EnemyExist(Vector2Int unitCords)
    {
        List<Unit> units = FindObjectOfType<UnitController>().Units;

        foreach (Unit unit in units)
        {
            if (unit.cords == unitCords)
            {
                return unit;
            }
        }
        return null;
    }
    private Unit ObstacleExist(Vector2Int blockCords)
    {
        List<Unit> obstacles = FindObjectOfType<UnitController>().Obstacles;

        foreach (Unit block in obstacles)
        {
            if (block.cords == blockCords)
            {
                return block;
            }
        }
        return null;
    }

    public void DealSlashAttack(int dmg)
    {
        List<Unit> units = FindObjectOfType<UnitController>().Units;
        List<Unit> obstacles = FindObjectOfType<UnitController>().Obstacles;

        // Check which units to destroy
        List<Unit> toDestroy = new List<Unit>();

        // Loop the attackable cords
        foreach (Vector2Int a in attackableCords)
        {
            // Check if there is enemy in the cords
            Unit u = EnemyExist(a);
            if (u != null)
            {
                u.health -= dmg;
                // if health after is zero or less than zero, add it to toDestroy List
                if (u.health <= 0)
                {
                    toDestroy.Add(u);
                }
                else
                {
                    UnitHealthBar unitHealthBar = u.unitGameObject.GetComponentInChildren<UnitHealthBar>();
                    if (unitHealthBar != null)
                    {
                        unitHealthBar.UpdateHealthbar((float)u.health, (float)u.maxHealth);
                    }
                }
            }
        }

        // Destroy unit gameObjects and remove it from the list of units
        foreach (Unit u in toDestroy)
        {
            // Destroy the gameObject
            Destroy(u.unitGameObject, 1);
            // Release the tile
            gridManager.ReleaseTile(u.cords);
            // Remove in array
            units.Remove(u);
        }

        foreach (Vector2Int b in attackableCords)
        {
            // Check if there is enemy in the cords
            Unit block = ObstacleExist(b);
            if (block != null)
            {
                block.health -= dmg;
                // if health after is zero or less than zero, add it to toDestroy List
                if (block.health <= 0)
                {
                    toDestroy.Add(block);
                }
                else
                {
                    // UnitHealthBar unitHealthBar = u.unitGameObject.GetComponentInChildren<UnitHealthBar>();
                    // if (unitHealthBar != null)
                    // {
                    //     unitHealthBar.UpdateHealthbar((float)u.health, (float)u.maxHealth);
                    // }
                }
            }
        }

        // Destroy unit gameObjects and remove it from the list of units
        foreach (Unit block in toDestroy)
        {
            // Destroy the gameObject
            Destroy(block.unitGameObject, 1);
            // Release the tile
            gridManager.ReleaseTile(block.cords);
            // Remove in array
            obstacles.Remove(block);
        }


        //UnityEngine.Debug.Log(units.Count);
        checkLevelOver();
    }

    public void ExecuteCard(){
        PopulateAttackArea("Basic");
        EnableAttackableTiles();
        isAttacking = true;
    }
    public void ConfirmExecuteCard(int dmg){
        // face the direction of the enemy to attack
        selectedUnit.LookAt(new Vector3(selectedLocation.x, selectedUnit.position.y, selectedLocation.y));
        StartCoroutine(WaitAndAnimate(1.0f, "isExecuteAttack"));
        StartCoroutine(audioManager.WaitAndPlaySFX(1.2f,"attack"));
        DisableAttackableTiles();
        Execute(dmg);
        isAttacking = false;
    }
    public void CancelExecuteCard(){
        DisableAttackableTiles();
        isAttacking = false;

    }

    public void Execute(int dmg){
        // Check which unit is selected
        List<Unit> units = FindObjectOfType<UnitController>().Units;
        List<Unit> obstacles = FindObjectOfType<UnitController>().Obstacles;


        // Check which units to destroy
        List<Unit> toDestroy = new List<Unit>();

        for (int i = 0; i < units.Count; i++)
        {
            Unit u = units[i];
            float threshold = u.maxHealth * 0.3f;

            if (u.cords == selectedLocation)
            {
                if(u.health<= threshold){
                    toDestroy.Add(u);
                }
                else{
                    u.health -= dmg;
                }
                // if health after is zero or less than zero, add it to toDestroy List
                if (u.health <= 0)
                {
                    toDestroy.Add(u);
                }
                else
                {
                    UnitHealthBar unitHealthBar = u.unitGameObject.GetComponentInChildren<UnitHealthBar>();
                    if (unitHealthBar != null)
                    {
                        unitHealthBar.UpdateHealthbar((float)u.health, (float)u.maxHealth);
                    }
                }
                break;
            }
        }

        foreach(Unit u  in toDestroy)
        {
            // Destroy the gameObject
            Destroy(u.unitGameObject, 1);
            // Release the tile
            gridManager.ReleaseTile(u.cords);
            // Remove in array
            units.Remove(u);
        }

        for (int i = 0; i < obstacles.Count; i++)
        {
            Unit block = obstacles[i];
            float threshold = block.maxHealth * 0.15f;

            if (block.cords == selectedLocation)
            {
                if(block.health<= threshold){
                    toDestroy.Add(block);
                }
                else{
                    block.health -= dmg;
                }
                // if health after is zero or less than zero, add it to toDestroy List
                if (block.health <= 0)
                {
                    toDestroy.Add(block);
                }
                else
                {
                    // UnitHealthBar unitHealthBar = u.unitGameObject.GetComponentInChildren<UnitHealthBar>();
                    // if (unitHealthBar != null)
                    // {
                    //     unitHealthBar.UpdateHealthbar((float)u.health, (float)u.maxHealth);
                    // }
                }
                break;
            }
        }

        foreach(Unit block  in toDestroy)
        {
            // Destroy the gameObject
            Destroy(block.unitGameObject, 1);
            // Release the tile
            gridManager.ReleaseTile(block.cords);
            // Remove in array
            obstacles.Remove(block);
        }

        // Find UnitController and make isDefeated true
        //UnityEngine.Debug.Log(units.Count);
        checkLevelOver();
    }

    private void checkLevelOver()
    {
        List<Unit> units = FindObjectOfType<UnitController>().Units;
        if (units.Count <= 0)
        {
            oldTile = null;
            currentTile = null;
            DisableMovableTiles();
            DisableAttackableTiles();
            RandomCardReward.AssignRandomCards();
        }
    }

    public void ShieldCard(int s)
    {
        // add shield
        shield += s;
        shieldSlider.maxValue = shield;
        

    }

    public void ConfirmShieldCard()
    {
        // increase player health for one round
        // UnityEngine.Debug.Log("shield:"+shield);
        GameObject casting = selectedUnit.Find("casting").gameObject;
        casting.SetActive(true);

    }

    public void CancelShieldCard(int s)
    {
        // decrease player health after one round
        shield -= s;
        shieldSlider.maxValue = shield;
   
    }

    public void HealCard(int h)
    {
        currentHealth = playerHealth;
        if (playerHealth >= maxPlayerHealth)
        {
            return;
        }
        else
        {
            // can heal any amount you want, now it is 2
            playerHealth += h;

            if (playerHealth > maxPlayerHealth)
            {
                playerHealth = maxPlayerHealth;
            }
        }
    }

    public void ConfirmHealCard()
    {
        // increase player health for one round
        audioManager.PlayHealing();

    }

    public void CancelHealCard()
    {
        playerHealth = currentHealth;
    }

    private void GetInRangeTiles()
    {
        movableCords.Clear();

        inRangeTiles = rangeFinder.GetTilesInRange(gridManager.GetTile(currentLocation), moveDistance);

        foreach (Tile item in inRangeTiles)
        {
            movableCords.Add(item.cords);
        }

        FilterMovableArea();
    }

    private void FilterMovableArea()
    {

        //Check and remove Coordinates that contains enemies
        List<Unit> units = FindObjectOfType<UnitController>().Units;

        foreach (Unit unit in units)
        {
            for (int i = 0; i < movableCords.Count; i++)
            {
                if (new Vector2Int((int)unit.unitGameObject.transform.position.x, (int)unit.unitGameObject.transform.position.z) == movableCords[i])
                {
                    movableCords.RemoveAt(i);
                }
            }
        }

        List<Vector2Int> tempCords = new List<Vector2Int>();

        for (int i = 0; i < movableCords.Count; i++)
        {
            List<Tile> testPath = pathFinder.findPath(gridManager.GetTile(currentLocation), gridManager.GetTile(movableCords[i]));
            if (testPath.Count <= moveDistance && testPath.Count != 0)
            {
                //UnityEngine.Debug.Log(movableCords[i] + " Count: " + testPath.Count);
                tempCords.Add(movableCords[i]);
            }
        }

        movableCords = tempCords;
        movableCords.Add(currentLocation);

        Tile topTile = gridManager.GetTile(new Vector2Int(currentLocation.x, currentLocation.y + 1));
        Tile bottomTile = gridManager.GetTile(new Vector2Int(currentLocation.x, currentLocation.y - 1));
        Tile leftTile = gridManager.GetTile(new Vector2Int(currentLocation.x - 1, currentLocation.y));
        Tile rightTile = gridManager.GetTile(new Vector2Int(currentLocation.x + 1, currentLocation.y));


        if (topTile != null && bottomTile != null && leftTile != null && rightTile != null)
        {
            if (topTile.isBlocked == true && bottomTile.isBlocked == true && leftTile.isBlocked == true && rightTile.isBlocked == true)
            {
                movableCords.Clear();
                movableCords.Add(currentLocation);

            }
        }


        //CheckMoveConnected();
    }

    private void PopulateAttackArea(string type)
    {
        attackableCords.Clear();

        int xCord = (int)selectedUnit.position.x;
        int yCord = (int)selectedUnit.position.z;

        if (type == "Basic")
        {
            //Top
            attackableCords.Add(new Vector2Int(xCord, yCord + 1));
            //Bottom
            attackableCords.Add(new Vector2Int(xCord, yCord - 1));
            //Left
            attackableCords.Add(new Vector2Int(xCord - 1, yCord));
            //Right
            attackableCords.Add(new Vector2Int(xCord + 1, yCord));
        }
        if (type == "Slash")
        {
            //Top
            attackableCords.Add(new Vector2Int(xCord, yCord + 1));
            //Bottom
            attackableCords.Add(new Vector2Int(xCord, yCord - 1));
            //Left
            attackableCords.Add(new Vector2Int(xCord - 1, yCord));
            //Right
            attackableCords.Add(new Vector2Int(xCord + 1, yCord));
            //Top-Right
            attackableCords.Add(new Vector2Int(xCord + 1, yCord + 1));
            //Top-Left
            attackableCords.Add(new Vector2Int(xCord - 1, yCord + 1));
            //Bottom-Right
            attackableCords.Add(new Vector2Int(xCord + 1, yCord - 1));
            //Bottom-Left
            attackableCords.Add(new Vector2Int(xCord - 1, yCord - 1));        
        }
    }


    private bool coordsExist(int x, int y)
    {

        if (0 > x || x >= gridManager.GridSize.x || 0 > y || y >= gridManager.GridSize.y)
        {
            return true;
        }

        for (int i = 0; i < movableCords.Count; i++)
        {
            if (movableCords[i].x == x && movableCords[i].y == y)
            {
                return true;
            }
        }

        return false;
    }

    private void EnableMovableTiles()
    {
        for (int i = 0; i < movableCords.Count; i++)
        {
            GameObject tileSteppedOn = GameObject.Find("(" + movableCords[i].x.ToString() + ", " + movableCords[i].y.ToString() + ")");

            if (tileSteppedOn != null)
            {
                tileSteppedOn.GetComponent<BoxCollider>().enabled = true;
                tileSteppedOn.GetComponentInChildren<MeshRenderer>().material = movableTile;
            }
        }
    }

    private void DisableMovableTiles()
    {
        GameObject playerTile = GameObject.Find("(" + selectedUnit.position.x.ToString() + ", " + selectedUnit.position.z.ToString() + ")");
        if (playerTile!=null){
            foreach(Transform child in playerTile.transform) {
                            child.GetComponentInChildren<MeshRenderer>().material.color = Color.white;
                        }
                playerTile.GetComponentInChildren<MeshRenderer>().material=inactiveTile;
        }
        for (int i = 0; i < movableCords.Count; i++)
        {
            GameObject tileSteppedOn = GameObject.Find("(" + movableCords[i].x.ToString() + ", " + movableCords[i].y.ToString() + ")");

            if (tileSteppedOn != null)
            {
                tileSteppedOn.GetComponent<BoxCollider>().enabled = false;
                tileSteppedOn.GetComponentInChildren<MeshRenderer>().material = inactiveTile;
            }
        }
        oldTile = null;
        currentTile = null;
    }

    private void EnableAttackableTiles()
    {
        for (int i = 0; i < attackableCords.Count; i++)
        {
            GameObject tileSteppedOn = GameObject.Find("(" + attackableCords[i].x.ToString() + ", " + attackableCords[i].y.ToString() + ")");

            if (tileSteppedOn != null)
            {
                tileSteppedOn.GetComponent<BoxCollider>().enabled = true;
                tileSteppedOn.GetComponentInChildren<MeshRenderer>().material = attackableTile;
            }
        }
    }

    private void DisableAttackableTiles()
    {
        GameObject playerTile = GameObject.Find("(" + selectedUnit.position.x.ToString() + ", " + selectedUnit.position.z.ToString() + ")");
        if (playerTile!=null){
            foreach(Transform child in playerTile.transform) {
                            child.GetComponentInChildren<MeshRenderer>().material.color = Color.white;
                        }
                playerTile.GetComponentInChildren<MeshRenderer>().material=inactiveTile;
        }
        for (int i = 0; i < attackableCords.Count; i++)
        {
            GameObject tileSteppedOn = GameObject.Find("(" + attackableCords[i].x.ToString() + ", " + attackableCords[i].y.ToString() + ")");

            if (tileSteppedOn != null)
            {
                tileSteppedOn.GetComponent<BoxCollider>().enabled = false;
                foreach(Transform child in tileSteppedOn.transform) {
                            child.GetComponentInChildren<MeshRenderer>().material.color = Color.white;
                        }
                tileSteppedOn.GetComponentInChildren<MeshRenderer>().material = inactiveTile;
            }
        }
        oldTile = null;
        currentTile = null;
    }

    // For animations
    IEnumerator WaitAndAnimate(float sec, string s)
    {
        animator.SetBool(s, true);
        yield return new WaitForSeconds(sec);
        animator.SetBool(s, false);
    }
}
