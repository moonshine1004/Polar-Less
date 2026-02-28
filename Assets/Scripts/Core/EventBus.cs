using System;
using System.Collections.Generic;
using UnityEngine;

public interface IEventBusSubscription
{
    IDisposable Subscribe<T>(Action<T> handler);
}
public interface IEventBusPublish
{
    void Publish<T>(T evt);
}

public class EventBus : IEventBusSubscription, IEventBusPublish
{   
    /// <summary>
    /// 키: 이벤트 타입
    /// 값: 해당 타입을 구독한 핸들러(메서드) 리스트
    /// </summary>
    private readonly Dictionary<Type, List<Delegate>> _map = new(); // 메서드를 담아둔 딕셔너리
    
    public IDisposable Subscribe<T>(Action<T> handler) // 구독
    {
        var type = typeof(T);
        if(!_map.TryGetValue(type, out var list)) // 딕셔너리 내에서 파라메터 타입에 해당하는 타입이 없으면
        {
            list = new List<Delegate>(); // 새로운 리스트 생성
            _map[type] = list; // 타입을 키로 새 리스트를 딕셔너리에 추가
        }
        list.Add(handler); // 리스트에 메서드 추가

        return new Subscription(() => Unsubscribe(handler)); 
    }
    
    public void Publish<T>(T evt) // 실행
    {
        var type = typeof(T);
        _map.TryGetValue(type, out var list);
        for(int i = 0; i < list.Count; i++)
        {
            var handler = (Action<T>)list[i];
            handler(evt);
        }
    }

    private void Unsubscribe<T>(Action<T> handler)
    {
        var type = typeof(T);
        if (!_map.TryGetValue(type, out var list))
            return;

        var idx = list.IndexOf(handler);
        if (idx >= 0) list.RemoveAt(idx);

        if (list.Count == 0)
            _map.Remove(type);
    }
    private class Subscription : IDisposable
    {
        private Action _dispose;
        public Subscription(Action dispose) => _dispose = dispose;

        public void Dispose()
        {
            _dispose?.Invoke(); // ★ 여기서 실제 Unsubscribe 수행
            _dispose = null;
        }
    }

    
}