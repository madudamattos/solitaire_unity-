using System.Collections.Generic;
using UnityEngine;

namespace Solitaire.Managers
{
    public interface ICommand
    {
        void Execute();
        void Undo();
    }

    public static class CommandManager
    {
        private static Stack<ICommand> _commandHistory = new Stack<ICommand>();
        
        private static float _lastCommandTime = 0f;
        private const float AnimationDuration = 0.25f; 
        public static bool IsProcessing => Time.time < _lastCommandTime + AnimationDuration;
        
        public static bool HasCommands => _commandHistory.Count > 0;
        
        public static void AddCommand(ICommand command)
        {
            if(IsProcessing) return;

            command.Execute();
            _commandHistory.Push(command);
        
            _lastCommandTime = Time.time;
        }

        public static void UndoLastCommand()
        {
            if(_commandHistory.Count == 0 || IsProcessing) return;
            
            ICommand lastCommand = _commandHistory.Pop();
            lastCommand.Undo();

            _lastCommandTime = Time.time;
        }

        public static void ClearHistory()
        {
            _commandHistory.Clear();
        }
    }
}