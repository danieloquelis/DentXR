using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LibCSG;
using System.Threading;

public class Sample_Move : MonoBehaviour
{
    [SerializeField]
    private Transform cylinderTransform;

    [SerializeField]
    private MeshFilter resultMeshFilter;

    CSGBrush cube;
    CSGBrush cylinder;
    CSGBrushOperation CSGOp = new CSGBrushOperation();
    CSGBrush cube_sub_cylinder;
    bool Move = false;
    float value_add;

    // Start is called before the first frame update
    void Start()
    {
        // Create the CSGBrushOperation
        CSGOp = new CSGBrushOperation();
        // Create the brush to contain the result of the operation
        // Give a GameObject allow to have the mesh result link with the GameObject Transform link
        // if yo don't give a GameObject the Brush create a new GameObject
        cube_sub_cylinder = new CSGBrush(GameObject.Find("Result"));

        value_add = -0.01f;
    }
    
    public void CreateBrush()
    {
        // Create the Brush for the cube
        cube = new CSGBrush(GameObject.Find("Cube"));
        // Set-up the mesh in the Brush
        cube.build_from_mesh(GameObject.Find("Cube").GetComponent<MeshFilter>().mesh);

        // Create the Brush for the cylinder
        cylinder = new CSGBrush(GameObject.Find("Cylinder"));
        // Set-up the mesh in the Brush
        cylinder.build_from_mesh(GameObject.Find("Cylinder").GetComponent<MeshFilter>().mesh);
    }
    
    public void StartMove()
    {
        Move=!Move;
    }

    // Update is called once per frame
    void Update()
    {
        if(Move){
            cylinderTransform.Translate(new Vector3(0,0,value_add));

            // Do the operation subtration between the cube and the cylinder 
            CSGOp.merge_brushes(Operation.OPERATION_SUBTRACTION, cube, cylinder, ref cube_sub_cylinder);

            resultMeshFilter.mesh.Clear();

            // Put the mesh result in the mesh give in parameter if you don't give a mesh he return a new mesh with the result
            cube_sub_cylinder.getMesh(resultMeshFilter.mesh);


            if(cylinderTransform.position.y>0.5f){
                value_add = -0.01f;
            }
            else if(cylinderTransform.position.y<-0.5f){
                value_add = 0.01f;
            }
        }
        
    }

}
