using CustomStateMachine;

namespace ThirdPersonPlayerController {
    public class ChangeCharacter : State {
        private Player _player;
        private int _currentCharacter;
        private int _characters;

        public ChangeCharacter(Player player, int characters) {
            _player = player;
            _characters = characters;
        }

        public override void Enter() {
            ++_currentCharacter;
            if (_currentCharacter == _characters) _currentCharacter = 0;
            _player.ChangeCharacterByIndex(_currentCharacter);
        }
    }
}
