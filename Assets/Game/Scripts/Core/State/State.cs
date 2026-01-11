namespace Eclipse
{
    public class State<T> where T : struct
    {
        public State(T id)
        {
            this.ID = id;
        }


        public T ID { get; private set; }

        public System.Action onEnter;
        public System.Action onExit;

        public void OnEnter()
        {
            if (onEnter != null)
            {
                onEnter();
            }
        }

        public void OnExecute() { }
        public void OnExit() 
        {
            if (onExit != null)
            { 
                onExit(); 
            }
            
        }
    }
}