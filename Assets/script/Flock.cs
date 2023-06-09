using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    //Craindo variáveis para o objeto que vai fazer o flocking
    public FlockManager myManager;
    public float speed;
    bool turning = false;
    void Start()
    {
        //Declarando a variável de velocidade para ser aleatório entre a velocidade mínima e máxima
        speed = Random.Range(myManager.minSpeed,
        myManager.maxSpeed);
    }

    void Update()
    {
        //Declarando variáveis para o objeto não sair do limite
        Bounds b = new Bounds(myManager.transform.position, myManager.swinLimits * 2);
        RaycastHit hit = new RaycastHit();
        Vector3 direction = myManager.transform.position - transform.position;
        //Verificando a área e declarando a direção
        if (!b.Contains(transform.position))
        {
            turning = true;
            direction = myManager.transform.position - transform.position;
        }
        else if (Physics.Raycast(transform.position, this.transform.forward * 50, out hit))
        {
            turning = true;
            direction = Vector3.Reflect(this.transform.forward, hit.normal);
        }
        //Se não tinver, não anda
        else
            turning = false;
        //Se o bool for ativado, o objeto anda
        if (turning)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation,
            Quaternion.LookRotation(direction),
            myManager.rotationSpeed * Time.deltaTime);
        }
        else
        {
            if (Random.Range(0, 100) < 10)
                speed = Random.Range(myManager.minSpeed,
                myManager.maxSpeed);
            if (Random.Range(0, 100) < 20)
                ApplyRules();
        }
        transform.Translate(0, 0, Time.deltaTime * speed);
    }

    //Criando um método para aplicar as regras
    void ApplyRules()
    {
        //Declarnado variáveis
        GameObject[] gos;
        gos = myManager.allFish;
        Vector3 vcentre = Vector3.zero;
        Vector3 vavoid = Vector3.zero;
        float gSpeed = 0.01f;
        float nDistance = 0;
        int groupSize = 0;
        //Criando uma estrutura de repetição
        foreach (GameObject go in gos)
        {
            //Se cada objeto for diferente dos outros, declara a distância
            if (go != this.gameObject)
            {
                nDistance = Vector3.Distance(go.transform.position, this.transform.position);
            }
            //Se a distância for menor que a distância que foi declarada, a distância de cada obejto é declarada
            if (nDistance <= myManager.neighbourDistance)
            {
                vcentre += go.transform.position;
                groupSize++;
                if (nDistance < 1.0f)
                {
                    vavoid = vavoid + (this.transform.position - go.transform.position);
                }
                Flock anotherFlock = go.GetComponent<Flock>();
                gSpeed = gSpeed + anotherFlock.speed;
            }
            //Se o grupo de objetos for menor que zero, a velocidade muda
            if (groupSize > 0)
            {
                vcentre = vcentre / groupSize + (myManager.goalPos - this.transform.position);
                speed = gSpeed / groupSize;
                Vector3 direction = (vcentre + vavoid) - transform.position;
                if (direction != Vector3.zero)
                    transform.rotation = Quaternion.Slerp(transform.rotation,
                    Quaternion.LookRotation(direction),
                    myManager.rotationSpeed * Time.deltaTime);
            }
        }
    }
}
