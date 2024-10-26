using UnityEngine;

[CreateAssetMenu(fileName = "Firestore Connection", menuName = "Firestore/Connection")]
public class FirestoreConnection : ScriptableObject
{
    public string apiKey;
    public string authDomain;
    public string projectId;
    public string storageBucket;
    public string messagingSenderId;
    public string appId;

    [Space]
    public string collectionName;
    public string documentId;
}