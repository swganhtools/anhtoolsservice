using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.ComponentModel;
using System.Security.Permissions;
using System.Diagnostics;

// © 2005 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net


[Serializable]
internal class WorkItem : IAsyncResult
{
   object[] m_Args;
   object m_AsyncState;
   bool m_Completed;
   Delegate m_Method;
   ManualResetEvent m_Event;
   object m_MethodReturnedValue;
   internal WorkItem(object AsyncState,Delegate method,object[] args)
   {
      m_AsyncState = AsyncState;
      m_Method = method;
      m_Args =  args;
      m_Event = new ManualResetEvent(false);
      m_Completed = false;
   }
   //IAsyncResult properties 
   object IAsyncResult.AsyncState
   { 
      get
      {
         return m_AsyncState;
      }
   }
   WaitHandle IAsyncResult.AsyncWaitHandle
   { 
      get
      {
         return m_Event;
      }
   }
   bool IAsyncResult.CompletedSynchronously
   {
      get
      {
         return false;
      }
   }
   bool IAsyncResult.IsCompleted
   { 
      get
      {
         return Completed;
      }
   }
   bool Completed
   {
      get
      {
         lock(this)
         {
            return m_Completed;
         }
      }
      set
      {
         lock(this)
         {
            m_Completed = value;
         }
      }
   }

   //This method is called on the worker thread to execute the method
   internal void CallBack()
   {
      MethodReturnedValue = m_Method.DynamicInvoke(m_Args);
      //Method is done. Signal the world
      m_Event.Set();
      Completed = true;
   }
   internal object MethodReturnedValue
   {
      get
      {
         lock(this)
         {
            return m_MethodReturnedValue;
         }
      }
      set
      {
         lock(this)
         {
            m_MethodReturnedValue = value;
         }
      }

   }
}
[SecurityPermission(SecurityAction.Demand,ControlThread = true)]
public class Synchronizer: ISynchronizeInvoke,IDisposable
{
   WorkerThread m_WorkerThread;
   public bool InvokeRequired 
   {
      get
      {
         bool res = Thread.CurrentThread.ManagedThreadId == m_WorkerThread.ManagedThreadId;
         return  ! res;
      }
   }
   public IAsyncResult BeginInvoke(Delegate method, object[] args)
   {
      WorkItem result = new WorkItem(null,method,args);
      m_WorkerThread.QueueWorkItem(result);
      return result;
   }
   public object EndInvoke(IAsyncResult result)
   {
      result.AsyncWaitHandle.WaitOne();
      WorkItem workItem = (WorkItem)result;
      return  workItem.MethodReturnedValue;
   }
   public object Invoke(Delegate method, object[] args)
   {
      IAsyncResult asyncResult;
      asyncResult = BeginInvoke(method,args);
      return EndInvoke(asyncResult);
   }
   public Synchronizer()
   {
      m_WorkerThread = new WorkerThread(this);
   }
   ~Synchronizer()
   {
   }
   public void Dispose()
   {
      m_WorkerThread.Kill();
   }
   internal protected class WorkerThread 
   {
      public Thread m_ThreadObj;
      bool m_EndLoop;
      Mutex m_EndLoopMutex;
      AutoResetEvent m_ItemAdded;
      Synchronizer m_Synchronizer;
      Queue<WorkItem> m_WorkItemQueue;

      public int ManagedThreadId
      {
         get
         {
            return m_ThreadObj.ManagedThreadId;
         }
      }

      internal void QueueWorkItem(WorkItem workItem)
      {
         lock(m_WorkItemQueue)
         {
            m_WorkItemQueue.Enqueue(workItem);
            m_ItemAdded.Set();
         }
      }
      internal WorkerThread(Synchronizer synchronizer)
      {
         m_Synchronizer = synchronizer;
         m_EndLoop = false;
         m_ThreadObj = null;
         m_EndLoopMutex = new Mutex();
         m_ItemAdded = new AutoResetEvent(false);
         m_WorkItemQueue = new Queue<WorkItem>();
         CreateThread(true);
      }
      bool EndLoop
      {
         set
         {
            m_EndLoopMutex.WaitOne();
            m_EndLoop = value;
            m_EndLoopMutex.ReleaseMutex();
         }
         get
         {
            bool result = false;
            m_EndLoopMutex.WaitOne();
            result = m_EndLoop;
            m_EndLoopMutex.ReleaseMutex();
            return result;
         }
      }
      Thread CreateThread(bool autoStart)
      {
         if(m_ThreadObj != null)
         {
            Debug.Assert(false);
            return m_ThreadObj;
         }
         m_ThreadObj = new Thread(Run);
         m_ThreadObj.Name = "Synchronizer Worker Thread";
         if(autoStart == true)
         {
            m_ThreadObj.Start();
         }
         return m_ThreadObj;
      }
      void Start()
      {
         Debug.Assert(m_ThreadObj != null);
         Debug.Assert(m_ThreadObj.IsAlive == false);
         m_ThreadObj.Start();
      }
      bool QueueEmpty
      {
         get
         {
            lock(m_WorkItemQueue)
            {
               if(m_WorkItemQueue.Count > 0)
               {
                  return false;
               }
               return true;
            }
         }
      }
      WorkItem GetNext()
      {
         if(QueueEmpty)
         {
            return null;
         }
         lock(m_WorkItemQueue)
         {
            return m_WorkItemQueue.Dequeue();
         }
      }
      void Run()
      {
         while(EndLoop == false)
         { 
            while(QueueEmpty == false)
            {
               if(EndLoop == true)
               {
                  return;
               }
               WorkItem workItem = GetNext();
               workItem.CallBack();
            }
            m_ItemAdded.WaitOne();
         }
      }
      public void Kill()
      { 
         //Kill is called on client thread - must use cached thread object
         Debug.Assert(m_ThreadObj != null);
         if(m_ThreadObj.IsAlive == false)
         {
            return;
         }
         EndLoop = true;
         m_ItemAdded.Set();

         //Wait for thread to die
         m_ThreadObj.Join();
         if(m_EndLoopMutex != null)
         {
            m_EndLoopMutex.Close();
         }
         if(m_ItemAdded != null)
         {
            m_ItemAdded.Close();
         }
      }
   }
}
