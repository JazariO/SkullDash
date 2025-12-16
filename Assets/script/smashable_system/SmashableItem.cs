using Proselyte.Sigils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SmashableItem : MonoBehaviour
{
    [SerializeField] float dissolveTime = 4f;

    [SerializeField] Renderer meshRenderer;
    [SerializeField] Collider smashable_collider;
    [SerializeField] Rigidbody body;
    [SerializeField] LayerMask groundMask;

    private Renderer[] renderers;
    private BoxCollider[] boxColliders;
    private Rigidbody[] rigidbodies;

    [SerializeField] GameEventDataSO game_event_data_SO;
    [SerializeField] InputDataSO input_data_SO;

    public GameObject smashedItem;

    [Serializable]
    public struct Data
    {
        public Renderer[] objectRenderers;
        public MaterialPropertyBlock[] propBlocks;
        public List<Mesh> meshes;
        public Material material;
    }

    internal Data smashedItemData;

    private bool isHit;
    private Vector3 contactPoint;
    private Vector3 forceDirection;

    private readonly int dissolveTimeID = Shader.PropertyToID("_DissolveTime");
    private bool already_smashed = false;

    private Vector3 startPos;
    private void Awake()
    {
        if(smashedItem == null) { Debug.LogWarning(gameObject.name + " has a SmashableItem Component without a reference to a smashed prefab"); return; };

        renderers = GetComponentsInChildren<Renderer>();
        boxColliders = GetComponentsInChildren<BoxCollider>();
        rigidbodies = GetComponentsInChildren<Rigidbody>();

        Renderer[] smashedRenderers =
        smashedItemData.objectRenderers = smashedItem.GetComponentsInChildren<Renderer>();

        smashedItemData.material = meshRenderer.sharedMaterial;
        MeshFilter[] meshFilters = smashedItem.GetComponentsInChildren<MeshFilter>();

        if(smashedItemData.meshes == null) smashedItemData.meshes = new List<Mesh>();

        for(int i = 0; i < meshFilters.Length; i++)
        {
            smashedItemData.meshes.Add(meshFilters[i].sharedMesh);
        }

        //smashedItemData.propBlocks = new MaterialPropertyBlock[smashedItemData.objectRenderers.Length];

        //for(int i = 0; i < smashedItemData.objectRenderers.Length; i++)
        //{
        //    smashedItemData.propBlocks[i] = new MaterialPropertyBlock();
        //}
    }

    private void OnEnable()
    {
        game_event_data_SO.OnGameReset.RegisterListener(ResetSmashable);
    }

    private void OnDisable()
    {
        game_event_data_SO.OnGameReset.UnregisterListener(ResetSmashable);
    }
    private void Start()
    {
        startPos = transform.position;
        foreach(var rb in rigidbodies) { if(rb != null) rb.isKinematic = true; }
    }
    private void FixedUpdate()
    {
        
        if(isHit && !already_smashed)
        {
            foreach(var renderer in renderers) { if(renderer != null) renderer.enabled = false; }

            foreach(var box in boxColliders) { if(box != null) box.enabled = false; }

            foreach(var rigidbody in rigidbodies) { if(rigidbody != null) rigidbody.isKinematic = true; }

            for(int i = 0; i < smashedItemData.meshes.Count; i++)
            {
                GameObject smashedItemClone = ObjectPoolManager.instance.smashedItemPool.Get();

                MeshFilter meshFilter = smashedItemClone.GetComponent<MeshFilter>();
                MeshRenderer meshRenderer = smashedItemClone.GetComponent<MeshRenderer>();
                Rigidbody body = smashedItemClone.GetComponent<Rigidbody>();
                body.mass = 20;
                MeshCollider collider = smashedItemClone.GetComponent<MeshCollider>();

                collider.convex = true;

                collider.sharedMesh = smashedItemData.meshes[i];

                meshRenderer.sharedMaterial = smashedItemData.material;


                smashedItemClone.transform.position = smashable_collider.transform.position + Vector3.up;

                body.isKinematic = false;
                meshFilter.sharedMesh = smashedItemData.meshes[i];

                body.AddForceAtPosition(-forceDirection * 10, contactPoint, ForceMode.Impulse);
                StartCoroutine(Dissolving(meshRenderer, body, smashedItemClone));
            }

            already_smashed = true;
        }
        else if(!Physics.Raycast(transform.position, Vector3.down, maxDistance: 0.1f, groundMask))
        {
            foreach(var rb in rigidbodies) { if(rb != null) rb.isKinematic = false; }
        }
    }

    public void Hit(Vector3 hit_contact_point, Vector3 hit_force_direction)
    {
        if(isHit) return;
        contactPoint = hit_contact_point;
        forceDirection = hit_force_direction;
        isHit = true;
    }

    private IEnumerator Dissolving(Renderer renderer, Rigidbody body, GameObject clone)
    {
        float elapsedTime = 0;

        while(elapsedTime < dissolveTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / dissolveTime;
            //SetMPBFloatVariable(renderer, dissolveTimeID, t);
            yield return null;
        }

        body.isKinematic = true;
        ObjectPoolManager.instance.smashedItemPool.Release(clone);
    }


    //protected void SetMPBFloatVariable(Renderer renderer, int valueID, float value)
    //{
    //    for(int i = 0; i < smashedItemData.objectRenderers.Length; i++)
    //    {
    //        renderer.GetPropertyBlock(smashedItemData.propBlocks[i]);
    //        smashedItemData.propBlocks[i].SetFloat(valueID, value);
    //        renderer.SetPropertyBlock(smashedItemData.propBlocks[i]);
    //    }
    //}

    private void ResetSmashable()
    {
        foreach(var renderer in renderers) { if(renderer != null) renderer.enabled = true; }

        foreach(var box in boxColliders) { if(box != null) box.enabled = true; }

        foreach(var rigidbody in rigidbodies)
        {
            if(rigidbody != null)
            {
                rigidbody.isKinematic = true;
                rigidbody.position = startPos;
            }
        }
    }
    //private void OnDrawGizmosSelected()
    //{
    //    if(smashable_collider == null) return;
    //    Vector3 worldCenter = smashable_collider.transform.TransformPoint(smashable_collider.bounds.center);
    //    Vector3 worldSize = Vector3.Scale(smashable_collider.bounds.size, smashable_collider.transform.lossyScale);

    //    Gizmos.matrix = Matrix4x4.TRS(worldCenter, smashable_collider.transform.rotation, Vector3.one);
    //    Gizmos.DrawWireCube(Vector3.zero, worldSize);
    //}
}
