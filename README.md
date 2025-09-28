# Chess Game in Unity

https://play.unity.com/en/games/d365c191-f73d-4be6-b345-f58ec4724add/chessgame
A fully-featured chess game developed in Unity with complete chess rules implementation and AI opponent using Minimax algorithm with Alpha-Beta pruning.

## Features

### Complete Chess Rules Implementation
- **Basic Movement**: All pieces move according to standard chess rules
- **Castling**: Both kingside and queenside castling with proper conditions
- **En Passant**: Capture of pawns that moved two squares on their first move
- **Pawn Promotion**: Automatic promotion to Queen for AI, manual selection for players
- **Check Detection**: Real-time check detection and validation
- **Checkmate**: Automatic game ending when king is in checkmate
- **Stalemate**: Draw detection when no legal moves are available
- **Threefold Repetition**: Draw by repetition tracking using position keys
- **Insufficient Material**: Draw when only kings remain on board

### AI Implementation
- **Minimax Algorithm**: Classic game tree search algorithm
- **Alpha-Beta Pruning**: Optimization for faster move calculation
- **Position Evaluation**: Material-based evaluation system
- **Configurable Depth**: Adjustable AI difficulty (depth 1-5)
- **Automatic Promotion**: AI automatically promotes pawns to Queens

### Game Modes
- **Player vs Player**: Local multiplayer mode
- **Player vs AI**: Single player against computer opponent

### Technical Features
- **Move Validation**: Complete legal move checking including self-check prevention
- **Visual Feedback**: Clear indication of valid moves and game state
- **Event System**: Decoupled architecture using Unity events
- **Singleton Pattern**: Centralized game management
- **Modular Design**: Separate piece movement classes for each piece type

## Architecture

### Core Components
- **BoardManager**: Manages board state, piece placement, and move execution
- **GameManager**: Controls game flow, modes, and AI integration
- **TurnManager**: Handles turn switching and game state
- **UIManager**: Manages user interface and game over screens
- **MinimaxAI**: AI opponent implementation
- **CheckController**: Check and checkmate detection
- **PromoteController**: Handles pawn promotion interface

### Piece System
- **IPieceMover Interface**: Common interface for all piece movement logic
- **Individual Movers**: Separate classes for Pawn, Rook, Bishop, Knight, Queen, King
- **Move Validation**: Each piece validates its own moves according to chess rules

## AI Details

The AI uses the Minimax algorithm with the following features:
- **Search Depth**: Configurable from 1-5 moves ahead
- **Alpha-Beta Pruning**: Reduces search space for better performance
- **Position Evaluation**: 
  - King: 900 points
  - Queen: 90 points
  - Rook: 50 points
  - Bishop/Knight: 30 points
  - Pawn: 10 points
- **Negamax Implementation**: Simplified minimax variant for cleaner code

## Installation

1. Clone the repository
2. Open the project in Unity (2021.3 or later recommended)
3. Open the main scene
4. Press Play to start the game

## Controls

- **Mouse**: Click and drag pieces to move them
- **UI Buttons**: Select game mode (Player vs Player / Player vs AI)
- **Promotion**: Click on desired piece when pawn reaches end rank (player only)


## License

This project is open source and available under the [MIT License](LICENSE).

---

Developed with Unity Engine featuring complete chess rule implementation and intelligent AI opponent.
