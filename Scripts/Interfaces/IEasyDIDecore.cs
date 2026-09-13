
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEasyDIDecore<T>
{
    T Decore { get; set; }
    T PrevDecore { get; set; }

    public void AddDecore(T newDecore)
    {
        if (newDecore as IEasyDIDecore<T> == this)
        {
            EasyDI.EasyDILog.LogError($"Can't add self to decore!");
            return;
        }
        var root = GetRoot();
        //Debug.Log($"root: {root.GetType()}");
        var oldDecore = (root as IEasyDIDecore<T>).Decore;
        (root as IEasyDIDecore<T>).Decore = newDecore;
        (newDecore as IEasyDIDecore<T>).PrevDecore = root;
        (newDecore as IEasyDIDecore<T>).Decore = oldDecore;

    }

    void RemoveThisDecore()
    {
        if (PrevDecore != null)
        {
            (PrevDecore as IEasyDIDecore<T>).Decore = Decore;
            PrevDecore = default;
        }

        if (Decore != null)
        {
            (Decore as IEasyDIDecore<T>).PrevDecore = PrevDecore;
        }

    }

    T GetRoot()
    {
        var c = (T)this;
        var visited = new HashSet<IEasyDIDecore<T>>();

        while (c != null)
        {
            var current = c as IEasyDIDecore<T>;
            if (current == null) break;

            // nếu đã gặp rồi thì coi như loop, trả về ngay
            if (!visited.Add(current))
            {
                EasyDI.EasyDILog.LogError("Loop decorator detected!");
                return c;
            }

            // nếu PrevDecore trỏ ngược lại chính c thì trả về c
            if (current.PrevDecore != null && current.PrevDecore.Equals(c))
            {
                return c;
            }

            // đi ngược lên PrevDecore
            if (current.PrevDecore != null)
            {
                c = current.PrevDecore;
            }
            else
            {
                break;
            }
        }

        return c;
    }


    /// <summary>
    /// Check from root to end.
    /// </summary>
    /// <param name="onPeek"></param>
    void ForeachDecore(Action<T> onPeek)
    {
        int countLoop = 0;
        var root = GetRoot();
        var nextCheck = root;
        var start = root;
        while (nextCheck != null)
        {
            onPeek?.Invoke(nextCheck);
            nextCheck = (nextCheck as IEasyDIDecore<T>).Decore;

            if (nextCheck != null)//tranh loop
            {
                if (nextCheck.Equals(start))//tranh loop
                {
                    countLoop++;
                }
                if (countLoop > 2)
                {
                    EasyDI.EasyDILog.LogError($"Loop decorator detected!");
                    break;

                }
            }
        }

    }

    IList<T> ToListDecore()
    {
        IList<T> list = new List<T>();
        ForeachDecore(_ => list.Add(_));
        return list;
    }
}
