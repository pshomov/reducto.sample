using System;
using System.Collections.Generic;

namespace Reducto.Sample.Tests
{
    public class LoggedAction<S>
    {
        public Action Action;
        public S StateAfter;
    }

    public class Logger<AppState>
    {
        private readonly List<LoggedAction<AppState>> history = new List<LoggedAction<AppState>>();

        public AppState FirstAction(Type action)
        {
            return history.Find(a => a.Action.GetType() == action).StateAfter;
        }
        public Middleware<AppState> logger()
        {
            return s => next => action =>
            {
                next(action);
                var after = s.GetState();
                history.Add(new LoggedAction<AppState>
                    {
                        Action = action,
                        StateAfter = after
                    });
            };
        }
    }

}

