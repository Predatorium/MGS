using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    #region Enum
    private enum Etat
    {
        Surveillance,
        Patrouille,
        Poursuite,
        Ballade,
        Alerte,
        Repositionnement,
    }

    private enum Type
    {
        Static,
        Patrol,
        Wonderer,
    }
    #endregion

    #region Inspector
    [SerializeField] Type type = Type.Static;
    [SerializeField] private Transform[] PatrolPoint;
    [SerializeField] private float TimeWatching = 0f;
    [SerializeField] private int MaxLife = 0;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Character character = null;
    [SerializeField] private float LenghtDetection = 7f;
    [SerializeField] private float AngleDetection = 0.7f;
    [SerializeField] private Ammo ammo = null;
    [SerializeField] private int damage = 1;
    [SerializeField] private AmmoInWorld Ammoprefabs = null;
    #endregion

    #region Private
    private Etat EtatActuel = Etat.Surveillance;
    private int CurrentPoint = 0;
    private Vector3 Destination;
    private float timeIsWatch = 0f;
    private float timeAlerte = 0f;
    private float timefire = 0f;
    private Vector3 basePos;
    private int Life = 0;
    private Quaternion StartAngle;
    #endregion


    // Start is called before the first frame update
    void Start()
    {
        Life = MaxLife;
        basePos = transform.position;
        StartAngle = transform.rotation;
        Destination = basePos;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale != 0f)
        {
            switch (EtatActuel)
            {
                case Etat.Surveillance:
                    Surveillance();
                    break;
                case Etat.Patrouille:
                    Patrouille();
                    break;
                case Etat.Poursuite:
                    Poursuite();
                    break;
                case Etat.Ballade:
                    Ballade();
                    break;
                case Etat.Alerte:
                    Alerte();
                    break;
                case Etat.Repositionnement:
                    Repositionnement();
                    break;
            }

            if (EtatActuel != Etat.Poursuite)
            {
                DetectionPlayer();
            }

            if (Life == 0 || transform.position.y < -30)
            {
                AmmoInWorld tmp = Instantiate(Ammoprefabs);
                tmp.transform.position = transform.position;
                Destroy(gameObject);
            }
        }
    }

    public void TakeDamages(int damages)
    {
        Life -= damages;

        if (Life <= 0)
        {
            Life = 0;
        }

        if (Life > MaxLife)
        {
            Life = MaxLife;
        }

        Vector3 PosPlayer = character.transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(PosPlayer, out hit, 1f, -1))
        {
            Destination = hit.position;
            EtatActuel = Etat.Alerte;
            transform.LookAt(PosPlayer);
        }
    }

    private void Repositionnement()
    {
        agent.SetDestination(basePos);

        if (Vector3.Distance(transform.position, basePos) < 0.1f)
        {
            EtatActuel = Etat.Surveillance;
            transform.rotation = StartAngle;
        }
    }

    private void Patrouille()
    {
        agent.SetDestination(PatrolPoint[CurrentPoint].position);

        if (Vector3.Distance(transform.position, PatrolPoint[CurrentPoint].position) < 0.1f)
        {
            CurrentPoint++;

            if (CurrentPoint >= PatrolPoint.Length)
            {
                CurrentPoint = 0;
            }

            EtatActuel = Etat.Surveillance;
        }
    }

    private void DetectionPlayer()
    {
        Vector3 PosPlayer = character.transform.position;
        if (Vector3.Distance(transform.position, PosPlayer) < LenghtDetection && Vector3.Dot(transform.forward, (PosPlayer - transform.position).normalized) >= AngleDetection)
        {
            RaycastHit hitray;
            if (Physics.Raycast(transform.position, (PosPlayer - transform.position).normalized, out hitray, LenghtDetection))
            {
                if (hitray.collider.gameObject == character.gameObject)
                {
                    EtatActuel = Etat.Alerte;
                    timeIsWatch = 0;
                }
            }
        }
    }

    private void Surveillance()
    {
        timeIsWatch += Time.deltaTime;

        if (timeIsWatch > TimeWatching)
        {
            timeIsWatch = 0;

            if (type == Type.Patrol)
            {
                EtatActuel = Etat.Patrouille;
            }
            else if (type == Type.Wonderer)
            {
                EtatActuel = Etat.Ballade;
            }
            else if ((type == Type.Static) && Vector3.Distance(transform.position, basePos) > 0.1f)
            {
                EtatActuel = Etat.Repositionnement;
            }
        }
    }

    private void Alerte()
    {
        timeAlerte += Time.deltaTime;

        if (timeAlerte > 1f)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 30f);

            foreach (Collider col in colliders)
            {
                Enemy enemy = col.gameObject.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.EtatActuel = Etat.Poursuite;
                    enemy.Destination = character.transform.position;
                }
            }

            EtatActuel = Etat.Poursuite;
        }
    }

    private void Poursuite()
    {
        Vector3 PosPlayer = character.transform.position;
        if (Vector3.Distance(transform.position, PosPlayer) < LenghtDetection * 2f && Vector3.Dot(transform.forward, (PosPlayer - transform.position).normalized) >= AngleDetection / 2f)
        {
            RaycastHit hitray;
            if (Physics.Raycast(transform.position, (PosPlayer - transform.position).normalized, out hitray, LenghtDetection * 2f))
            {
                if (hitray.collider.gameObject == character.gameObject)
                {
                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(PosPlayer, out hit, 1f, -1))
                    {
                        Destination = hitray.point;
                        transform.LookAt(PosPlayer);

                        Collider[] colliders = Physics.OverlapSphere(transform.position, 30f);
                        foreach (Collider col in colliders)
                        {
                            Enemy enemy = col.gameObject.GetComponent<Enemy>();
                            if (enemy != null)
                            {
                                enemy.Destination = Destination;
                            }
                        }
                    }
                }
            }
        }

        Debug.DrawRay(transform.position, transform.forward * LenghtDetection);

        if (Vector3.Distance(transform.position, PosPlayer) < LenghtDetection)
        {
            RaycastHit hitray2;
            if (Physics.Raycast(transform.position, transform.forward, out hitray2, LenghtDetection))
            {
                if (hitray2.collider.gameObject == character.gameObject)
                {
                    timefire += Time.deltaTime;
                    agent.SetDestination(transform.position);

                    if (timefire > 1f)
                    {
                        Shoot();
                        timefire = 0;
                    }
                }
            }
        }
        else
        {
            agent.SetDestination(Destination);
            timefire = 0;
        }


        if (Vector3.Distance(Destination, PosPlayer) > 0.1f && Vector3.Distance(transform.position, Destination) < 0.5f)
        {
            EtatActuel = Etat.Surveillance;
        }
    }

    private void Ballade()
    {
        agent.SetDestination(Destination);

        if (Vector3.Distance(transform.position, Destination) < 1f)
        {
            for (int i = 0; i < 4; i++)
            {
                Destination = transform.position + (transform.forward * Random.Range(0, 7) + transform.right * Random.Range(-10, 10));
                
                NavMeshHit hit;
                if (NavMesh.SamplePosition(Destination, out hit, 1f, -1))
                {
                    Destination = hit.position + new Vector3(0f, 1f, 0f);
                    break;
                }
                else
                {
                    Destination = transform.position + (transform.forward * -2 + transform.right * Random.Range(-2, 2));
                }
            }

            EtatActuel = Etat.Surveillance;
        }
    }

    private void Shoot()
    {
        Ammo tmpAmmo = Instantiate(ammo);
        tmpAmmo.Damage = damage;
        tmpAmmo.transform.position = transform.position + transform.forward;
        tmpAmmo.transform.LookAt(tmpAmmo.transform.position + transform.forward);
        tmpAmmo.transform.Rotate(new Vector3(90f, 0f, 0f));
        tmpAmmo.Body.AddForce(transform.forward * 10f, ForceMode.Impulse);
    }
}
