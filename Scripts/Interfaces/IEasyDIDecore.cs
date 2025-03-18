
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
        var oldDecore = Decore as IEasyDIDecore<T>;
        Decore = newDecore;
        (newDecore as IEasyDIDecore<T>).PrevDecore = (T)this;
        (newDecore as IEasyDIDecore<T>).Decore = (T)oldDecore;
        if (oldDecore != null)
            oldDecore.PrevDecore = newDecore;
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
        List<IEasyDIDecore<T>> visited = new List<IEasyDIDecore<T>>();
        while (c != null)
        {
            if (!visited.Contains(c as IEasyDIDecore<T>))
                visited.Add(c as IEasyDIDecore<T>);
            else
            {
                visited.Add(c as IEasyDIDecore<T>);
                EasyDI.EasyDILog.LogError($"Loop decorator detected!");
                EasyDI.EasyDILog.LogError($"    decorator list: ");
                foreach (var item in visited)
                {
                    EasyDI.EasyDILog.LogError($"   {item.GetType()} hash: {item.GetHashCode()}");
                }
                break;
            }
            if ((c as IEasyDIDecore<T>).PrevDecore != null)
                c = (c as IEasyDIDecore<T>).PrevDecore;
            if ((c as IEasyDIDecore<T>) == this)
                break;
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
