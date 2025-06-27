using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LibCSG;

public class Sample : MonoBehaviour
{
    CSGBrush cube;
    CSGBrush sphere;
    CSGBrush cylinder;
    CSGBrushOperation CSGOp = new CSGBrushOperation();
    CSGBrush cube_inter_sphere;
    CSGBrush finalres;
    bool Move = false;
    float value_add;

    // Start is called before the first frame update
    void Start()
    {
        // Create the CSGBrushOperation
        CSGOp = new CSGBrushOperation();
        // Create the brush to contain the result of the operation cube_inter_sphere
        // You can give a name if you want a specifique name for the GameObject created
        cube_inter_sphere = new CSGBrush("cube_inter_sphere");
        // Create the brush to contain the result of another operation 
        finalres = new CSGBrush(GameObject.Find("Result"));
    }
    
    public void CreateBrush()
    {
        // Create the Brush for the cube
        cube = new CSGBrush(GameObject.Find("Cube"));
        // Set-up the mesh in the Brush
        cube.build_from_mesh(GameObject.Find("Cube").GetComponent<MeshFilter>().mesh);

        // Create the Brush for the cube
        sphere = new CSGBrush(GameObject.Find("Sphere"));
        // Set-up the mesh in the Brush
        sphere.build_from_mesh(GameObject.Find("Sphere").GetComponent<MeshFilter>().mesh);

        // Create the Brush for the cylinder
        cylinder = new CSGBrush(GameObject.Find("Cylinder"));
        // Set-up the mesh in the Brush
        cylinder.build_from_mesh(GameObject.Find("Cylinder").GetComponent<MeshFilter>().mesh);
    }
    
    public void CreateObjet()
    {

        // Do the operation intersection between the cube and the sphere 
        CSGOp.merge_brushes(Operation.OPERATION_INTERSECTION, cube, sphere, ref cube_inter_sphere);

        // Do the operation subtraction between the previous operation and the cylinder 
        CSGOp.merge_brushes(Operation.OPERATION_SUBTRACTION, cube_inter_sphere, cylinder, ref finalres);

        GameObject.Find("Result").GetComponent<MeshFilter>().mesh.Clear();

        // Put the mesh result in the mesh give in parameter if you don't give a mesh he return a new mesh with the result
        finalres.getMesh(GameObject.Find("Result").GetComponent<MeshFilter>().mesh);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
