using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 저울의 한쪽 플랫폼. ScaleWeightSource로부터 등록/해제를 받아 무게를 합산한다.
/// 감지 로직은 없음 — 오브젝트 쪽에서 발판을 찾아 직접 통보.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class ScalePlatform : MonoBehaviour
{
    private readonly Dictionary<ScaleWeightSource, int> _contributors = new();

    public int TotalWeight
    {
        get
        {
            int total = 0;
            foreach (int w in _contributors.Values) total += w;
            return total;
        }
    }

    public void Register(ScaleWeightSource source, int weight)
    {
        _contributors[source] = weight;
    }

    public void Unregister(ScaleWeightSource source)
    {
        _contributors.Remove(source);
    }
}
