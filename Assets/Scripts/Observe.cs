using System.Threading.Tasks;
using UnityEngine;

public class Observe : MonoBehaviour
{
    public int target_number;
    //private Quaternion initialRotation;
    private Other_Node other_node_U;
    private Other_Node other_node_C;

    //private float referenceX;
    //private float referenceY;
    //private float referenceZ;

    Main_Node main_Node;

    Posture_Indicator color_changer;

    GameObject variable_tracker = null;
    Variable_Tracker tracker = null;

    void Start()
    {

        variable_tracker = GameObject.Find("VariableTracker");
        if (variable_tracker != null)
        {
            tracker = variable_tracker.GetComponent<Variable_Tracker>();
        }
        else
        {
            Debug.LogWarning("GameObject with VariableTracker not found.");
        }

        GameObject reference = GameObject.Find("WriteFile");
        if (reference != null)
        {
            main_Node = reference.GetComponent<Main_Node>();
        }
        else
        {
            Debug.LogWarning("GameObject with WriteFile not found.");
        }

        GameObject posture = GameObject.Find("Posture_Indicator");
        if (posture != null)
        {
            color_changer = posture.GetComponent<Posture_Indicator>();
        }
        else
        {
            Debug.LogWarning("GameObject with Posture_Indicator not found.");
        }


        //initialRotation = transform.rotation;
        Other_Node[] all_nodes = FindObjectsOfType<Other_Node>();

        foreach (Other_Node node in all_nodes)
        {
            if (node.number == target_number && node.objName == "Write")
            {
                other_node_U = node;
                break;
            }
        }

        foreach (Other_Node node in all_nodes)
        {
            if (node.number == target_number && node.objName == "WriteFile")
            {
                other_node_C = node;
                break;
            }
        }




        if (other_node_U == null)
        {
            Debug.LogError("No Other_Node_U component was found. Check your target_number or ensure Other_Node is correctly assigned.");
        }
        else
        {
            Debug.Log($"Other_Node_U with number {target_number} found and assigned.");
        }

        if (other_node_C == null)
        {
            Debug.LogError("No Other_Node_C component was found. Check your target_number or ensure Other_Node is correctly assigned.");
        }
        else
        {
            Debug.Log($"Other_Node_C with number {target_number} found and assigned.");
        }
    }

    void Update()
    {
        //Quaternion currentRotation = transform.rotation;
        CheckJointAngles();
        //Debug.Log($"current rotate angle (each frame) - X: {currentRotation.eulerAngles.x}, Y: {currentRotation.eulerAngles.y}, Z: {currentRotation.eulerAngles.z}");
    }

    private void CheckJointAngles()
    {
        if (other_node_U == null)
        {
            Debug.LogError("other_node is null, cannot check joint angles.");
            return;
        }

        if (other_node_C == null)
        {
            Debug.LogError("other_node is null, cannot check joint angles.");
            return;
        }

        if (tracker.mode == 1) { 
            if (Mathf.Abs(other_node_C.delta_x - other_node_U.delta_x) > 30.0f || Mathf.Abs(other_node_C.delta_y - other_node_U.delta_y) > 30.0f || Mathf.Abs(other_node_C.delta_z - other_node_U.delta_z) > 30.0f)
            {
                Debug.Log($"{other_node_U.gameObject.name} should rotated by {other_node_C.delta_x - other_node_U.delta_x} degrees on the x-axis, {other_node_C.delta_y - other_node_U.delta_y} degrees on the y-axis, and {other_node_C.delta_z - other_node_U.delta_z} degrees on the z-axis");
                //Debug.Log($"{gameObject.name}Observe angles: x: {referenceX}, y: {referenceY}, z: {referenceZ}");
                //Debug.Log($"{other_node.gameObject.name} Other_Node angles: x: {other_node.delta_x}, y: {other_node.delta_y}, z: {other_node.delta_z}");
                main_Node.readFile = false;
                color_changer.set_color(other_node_U.gameObject.name);

            }
            else
            {
                Debug.Log("Aligned with");
                main_Node.readFile = true;
                color_changer.reset_color(other_node_U.gameObject.name);
            }
        }
        if (tracker.mode == 2)
        {
            if (Mathf.Abs(other_node_C.delta_x - other_node_U.delta_x) > 30.0f || Mathf.Abs(other_node_C.delta_y - other_node_U.delta_y) > 30.0f || Mathf.Abs(other_node_C.delta_z - other_node_U.delta_z) > 30.0f)
            {
                Debug.Log($"{other_node_U.gameObject.name} should rotated by {other_node_C.delta_x - other_node_U.delta_x} degrees on the x-axis, {other_node_C.delta_y - other_node_U.delta_y} degrees on the y-axis, and {other_node_C.delta_z - other_node_U.delta_z} degrees on the z-axis");
                //Debug.Log($"{gameObject.name}Observe angles: x: {referenceX}, y: {referenceY}, z: {referenceZ}");
                //Debug.Log($"{other_node.gameObject.name} Other_Node angles: x: {other_node.delta_x}, y: {other_node.delta_y}, z: {other_node.delta_z}");
                tracker.ticks++;
                main_Node.readFile = true;
                color_changer.set_color(other_node_U.gameObject.name);

            }
            else
            {
                Debug.Log("Aligned with");
                main_Node.readFile = true;
                color_changer.reset_color(other_node_U.gameObject.name);
            }
        }

    }

    //private Vector3 DeltaAngle_Calculation(Quaternion currentRotation)
    //{
    //    Quaternion deltaRotation = currentRotation * Quaternion.Inverse(initialRotation);
    //    Vector3 deltaAngles = deltaRotation.eulerAngles;

    //    return deltaAngles;
    //}

    private void Apply_Difference(Vector3 deltaAngles)
    {
        //switch (target_number)
        //{
        //    //left arm
        //    case > 6 and < 10:
        //        referenceX = Mathf.DeltaAngle(0, deltaAngles.z);
        //        referenceY = Mathf.DeltaAngle(0, deltaAngles.y);
        //        referenceZ = Mathf.DeltaAngle(0, deltaAngles.x);
        //        break;
        //    //right arm
        //    case > 9 and < 13:
        //        referenceX = (-1) * Mathf.DeltaAngle(0, deltaAngles.z);
        //        referenceY = (-1) * Mathf.DeltaAngle(0, deltaAngles.y);
        //        referenceZ = (-1) * Mathf.DeltaAngle(0, deltaAngles.x);
        //        break;


        //}

        //referenceX = Mathf.Clamp(referenceX, -90, 90);
        //referenceY = Mathf.Clamp(referenceY, -90, 90);
        //referenceZ = Mathf.Clamp(referenceZ, -90, 90);

        //Debug.Log($"{other_node.gameObject.name} should rotated by {referenceX - other_node.delta_x} degrees on the x-axis, {referenceY - other_node.delta_y} degrees on the y-axis, and {referenceZ - other_node.delta_z} degrees on the z-axis");

        //if (Mathf.Abs(referenceX - other_node.delta_x) > 5.0f || Mathf.Abs(referenceY - other_node.delta_y) > 5.0f || Mathf.Abs(referenceZ - other_node.delta_z) > 5.0f)
        //{
        //    Debug.Log($"{other_node.gameObject.name} should rotated by {referenceX - other_node.delta_x} degrees on the x-axis, {referenceY - other_node.delta_y} degrees on the y-axis, and {referenceZ - other_node.delta_z} degrees on the z-axis");
        //    //Debug.Log($"{gameObject.name}Observe angles: x: {referenceX}, y: {referenceY}, z: {referenceZ}");
        //    //Debug.Log($"{other_node.gameObject.name} Other_Node angles: x: {other_node.delta_x}, y: {other_node.delta_y}, z: {other_node.delta_z}");

        //}
        //else
        //{
        //    Debug.Log("Aligned with");
        //}

        //if (Mathf.Abs(deltaAngles.x - real_time_other.convertX) > 5.0f || Mathf.Abs(deltaAngles.y - real_time_other.convertY) > 5.0f || Mathf.Abs(deltaAngles.z - real_time_other.convertZ) > 5.0f)
        //{
        //    Debug.Log($"{gameObject.name} rotated by {deltaAngles.x - real_time_other.convertX} degrees on the x-axis, {deltaAngles.y - real_time_other.convertY} degrees on the y-axis, and {deltaAngles.z - real_time_other.convertZ} degrees on the z-axis");

        //}
        //if (gameObject.name == "mixamorig7:LeftArm" || gameObject.name == "mixamorig7:LeftForeArm" || gameObject.name == "mixamorig7:LeftHand")
        //{
        //    Debug.Log($"{gameObject.name} Observe angles: x: {referenceX}, y: {referenceY}, z: {referenceZ}");
        //    Debug.Log($"Other_Node angles: x: {other_node.convertX}, y: {other_node.convertY}, z: {other_node.convertZ}");
        //}
    }
}
