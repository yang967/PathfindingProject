using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detection
{

    public static bool detect(Vector3 position, float distance, LayerMask ignore)
    {
        RaycastHit hit;
        Ray ray = new Ray();

        Vector3 start = position + new Vector3(Mathf.Sin(60 * Mathf.Deg2Rad), 0, Mathf.Cos(60 * Mathf.Deg2Rad)) * distance;
        ray.origin = start;
        ray.direction = new Vector3(Mathf.Sin(-120 * Mathf.Deg2Rad), 0, Mathf.Cos(-120 * Mathf.Deg2Rad));
        //Debug.DrawRay(ray.origin, ray.direction * 2 * distance, Color.red, 20);
        if (Physics.Raycast(ray, out hit, 2 * distance, ~ignore))
            return true;

        ray.origin = position + new Vector3(Mathf.Sin(120 * Mathf.Deg2Rad), 0, Mathf.Cos(120 * Mathf.Deg2Rad)) * distance;
        ray.direction = new Vector3(Mathf.Sin(-60 * Mathf.Deg2Rad), 0, Mathf.Cos(-60 * Mathf.Deg2Rad));
        //Debug.DrawRay(ray.origin, ray.direction * 2 * distance, Color.red, 20);
        if (Physics.Raycast(ray, out hit, 2 * distance, ~ignore))
            return true;

        ray.origin = position + new Vector3(Mathf.Sin(180 * Mathf.Deg2Rad), 0, Mathf.Cos(180 * Mathf.Deg2Rad)) * distance;
        ray.direction = Vector3.forward; 
        //Debug.DrawRay(ray.origin, ray.direction * 2 * distance, Color.red, 20);
        if (Physics.Raycast(ray, out hit, 2 * distance, ~ignore))
            return true;

        return false;
    }
}
