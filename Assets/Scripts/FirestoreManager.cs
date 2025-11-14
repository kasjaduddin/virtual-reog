using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Firestore;
using UnityEngine;

public class FirestoreManager : IDataService
{
    private FirebaseFirestore db;

    public FirestoreManager()
    {
        if (FirebaseManager.IsReady)
        {
            db = FirebaseFirestore.DefaultInstance;
        }
        else
        {
            FirebaseManager.OnFirebaseReady += () => {
                db = FirebaseFirestore.DefaultInstance;
            };
            FirebaseManager.OnFirebaseError += (err) => {
                Debug.LogError($"[FirestoreManager] Firebase init error: {err}");
            };
        }
    }

    public async Task AddAsync<T>(string path, T data)
    {
        try
        {
            await db.Collection(path).AddAsync(data);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[FirestoreManager] AddAsync failed: {ex.Message}");
        }
    }

    public async Task<T> GetAsync<T>(string path)
    {
        try
        {
            var doc = await db.Document(path).GetSnapshotAsync();
            if (doc.Exists) return doc.ConvertTo<T>();
            Debug.LogWarning($"[FirestoreManager] Document not found at {path}");
            return default;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[FirestoreManager] GetAsync failed: {ex.Message}");
            return default;
        }
    }

    public async Task<IEnumerable<T>> QueryAsync<T>(string path, QueryOptions options = null)
    {
        try
        {
            Query query = db.Collection(path);

            if (options != null)
            {
                foreach (var filter in options.Filters)
                {
                    query = query.WhereEqualTo(filter.Key, filter.Value);
                }
                if (!string.IsNullOrEmpty(options.OrderBy))
                {
                    query = options.Descending
                        ? query.OrderByDescending(options.OrderBy)
                        : query.OrderBy(options.OrderBy);
                }
                if (options.Limit.HasValue)
                {
                    query = query.Limit(options.Limit.Value);
                }
            }

            var snapshot = await query.GetSnapshotAsync();
            List<T> results = new();
            foreach (var doc in snapshot.Documents)
            {
                results.Add(doc.ConvertTo<T>());
            }
            return results;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[FirestoreManager] QueryAsync failed: {ex.Message}");
            return new List<T>();
        }
    }

    public async Task UpdateAsync<T>(string path, T data)
    {
        try
        {
            await db.Document(path).SetAsync(data, SetOptions.MergeAll);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[FirestoreManager] UpdateAsync failed: {ex.Message}");
        }
    }

    public async Task DeleteAsync(string path)
    {
        try
        {
            await db.Document(path).DeleteAsync();
        }
        catch (Exception ex)
        {
            Debug.LogError($"[FirestoreManager] DeleteAsync failed: {ex.Message}");
        }
    }
}