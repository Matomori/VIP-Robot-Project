using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiskSite : MonoBehaviour
{
    [SerializeField] private RobotDogActivityManager manager;   //TODO: better way to do this?
    [SerializeField] private RiskIdentityMenu riskIdentityMenu;

    void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out DogController robotDog)) {
            riskIdentityMenu.CreateMenu(new List<string> { "test1", "test2", "test2" }, 2);
            riskIdentityMenu.gameObject.SetActive(true);
        }
    }
}
