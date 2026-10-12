using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfApp10
{
     
    public partial class MainWindow : Window
    {

        private const int BoardSize = 8;
        private const int PreviewSize = 2;
        private readonly int[,] boardData = new int[BoardSize, BoardSize];
        private readonly Button[,] boardButtons = new Button[BoardSize, BoardSize];

        private readonly List<(int row, int col)>[] currentBlocks = new List<(int, int)>[3];
        private readonly int[] slotShapeIndex = new int[3];
        private readonly bool[] slotUsed = new bool[3];
        private int selectedSlot = -1;
        private int score;
        private bool gameOver;
        private readonly Random random = new Random();
        private static readonly (int row, int col)[][] ShapeLibrary =
        {
        new[] { (0, 0) },
        new[] { (0, 0), (1, 0) },
        new[] { (0, 0), (0, 1), (1, 0), (1, 1) },
    };
        private static readonly Brush[] ShapeBrushes =
        {
        Freeze(52, 152, 219),
        Freeze(46, 204, 113),
        Freeze(155, 89, 182),
    };

        private static readonly Brush EmptyBrush = Freeze(236, 240, 241);
        private static readonly Brush PlacedBrush = Brushes.Orange;
        private static readonly Brush IdleBorderBrush = Freeze(149, 165, 166);

        public MainWindow()
        {
            InitializeComponent();
            CreateBoard();
            GenerateNewBlocks();
            UpdateScore();
        }
        private void GenerateNewBlocks()
        {
            for (int i = 0; i < 3; i++)
            {
                int shapeIndex = random.Next(ShapeLibrary.Length);
                slotShapeIndex[i] = shapeIndex;
                currentBlocks[i] = ShapeLibrary[shapeIndex].ToList();
                slotUsed[i] = false;

                Border slot = GetSlot(i);
                slot.Visibility = Visibility.Visible;
                slot.BorderBrush = IdleBorderBrush;
                slot.BorderThickness = new Thickness(3);

                DrawPreview(GetPreview(i), currentBlocks[i], ShapeBrushes[shapeIndex]);
            }

            selectedSlot = -1;
        }
        private void CreateBoard()
        {
            gameBoard.Children.Clear();

            for (int row = 0; row < BoardSize; row++)
            {
                for (int col = 0; col < BoardSize; col++)
                {
                    boardData[row, col] = 0;

                    var cell = new Button
                    {
                        Style = (Style)FindResource("CellButton"),
                        Background = EmptyBrush,
                        Tag = new Tuple<int, int>(row, col),
                    };
                    cell.Click += BoardButton_Click;

                    boardButtons[row, col] = cell;
                    gameBoard.Children.Add(cell);
                }
            }
        }
        private void DrawPreview(UniformGrid preview, List<(int row, int col)> shape, Brush color)
        {
            preview.Children.Clear();

            var occupied = new bool[PreviewSize, PreviewSize];
            foreach (var cell in shape)
            {
                occupied[cell.row, cell.col] = true;
            }

            for (int row = 0; row < PreviewSize; row++)
            {
                for (int col = 0; col < PreviewSize; col++)
                {
                    preview.Children.Add(new Border
                    {
                        Margin = new Thickness(3),
                        CornerRadius = new CornerRadius(4),
                        Background = occupied[row, col] ? color : Brushes.Transparent,
                    });
                }
            }
        }
        private void UpdateScore()
        {
            txtScore.Text = $"Điểm: {score}";
        }

        private Border GetSlot(int index)
        {
            if (index == 0)
            {
                return slot0;
            }

            if (index == 1)
            {
                return slot1;
            }

            return slot2;
        }

        private UniformGrid GetPreview(int index)
        {
            if (index == 0)
            {
                return preview0;
            }

            if (index == 1)
            {
                return preview1;
            }

            return preview2;
        }

        private static SolidColorBrush Freeze(byte red, byte green, byte blue)
        {
            var brush = new SolidColorBrush(Color.FromRgb(red, green, blue));
            brush.Freeze();
            return brush;
        }
        private void Slot_Click(object sender, MouseButtonEventArgs e)
        {
            if (gameOver)
            {
                return;
            }

            var slot = (Border)sender;
            int index = Convert.ToInt32(slot.Tag);

            if (slotUsed[index])
            {
                return;
            }

            selectedSlot = index;
            HighlightSelectedSlot();
            txtMessage.Text = "Đã chọn khối. Bấm một ô trên bàn cờ để đặt góc trái-trên của khối.";
        }

        private void HighlightSelectedSlot()
        {
            for (int i = 0; i < 3; i++)
            {
                if (slotUsed[i])
                {
                    continue;
                }

                Border slot = GetSlot(i);
                if (i == selectedSlot)
                {
                    slot.BorderBrush = Brushes.Gold;
                    slot.BorderThickness = new Thickness(5);
                }
                else
                {
                    slot.BorderBrush = IdleBorderBrush;
                    slot.BorderThickness = new Thickness(3);
                }
            }
        }
        private bool CanPlace(int slotIndex, int startRow, int startCol)
        {
            foreach (var cell in currentBlocks[slotIndex])
            {
                int row = startRow + cell.row;
                int col = startCol + cell.col;

                if (row < 0 || row >= BoardSize || col < 0 || col >= BoardSize)
                {
                    return false;
                }

                if (boardData[row, col] == 1)
                {
                    return false;
                }
            }

            return true;
        }
        private void PlaceBlock(int slotIndex, int startRow, int startCol)
        {
            foreach (var cell in currentBlocks[slotIndex])
            {
                int row = startRow + cell.row;
                int col = startCol + cell.col;
                boardData[row, col] = 1;
                boardButtons[row, col].Background = PlacedBrush;
            }

            slotUsed[slotIndex] = true;
            GetSlot(slotIndex).Visibility = Visibility.Collapsed;
        }
        private void BoardButton_Click(object sender, RoutedEventArgs e)
        {
            if (gameOver)
            {
                return;
            }

            if (selectedSlot < 0)
            {
                txtMessage.Text = "Hãy chọn một khối ở bên dưới trước.";
                return;
            }

            var button = (Button)sender;
            var position = (Tuple<int, int>)button.Tag!;
            int row = position.Item1;
            int col = position.Item2;

            if (!CanPlace(selectedSlot, row, col))
            {
                txtMessage.Text = "Không đặt được: khối tràn ra ngoài hoặc đè lên ô đã có.";
                return;
            }

            PlaceBlock(selectedSlot, row, col);
            int clearedLines = CheckAndClearLines();

            // Xóa hàng trước, rồi mới phát khối mới, để khối mới thấy những ô vừa trống.
            if (AllSlotsUsed())
            {
                GenerateNewBlocks();
            }
            else
            {
                selectedSlot = -1;
                HighlightSelectedSlot();
            }

            if (!HasAnyMove())
            {
                gameOver = true;
                txtMessage.Text = clearedLines > 0
                    ? $"Xóa {clearedLines} hàng/cột. Hết nước đi! Bấm Chơi lại."
                    : "Hết nước đi! Bấm Chơi lại để chơi ván mới.";
            }
            else if (clearedLines > 0)
            {
                txtMessage.Text = $"Xóa {clearedLines} hàng/cột. Cộng {clearedLines * 10} điểm.";
            }
            else
            {
                txtMessage.Text = "Đặt khối thành công.";
            }

            UpdateScore();
        }

        private bool AllSlotsUsed()
        {
            for (int i = 0; i < 3; i++)
            {
                if (!slotUsed[i])
                {
                    return false;
                }
            }

            return true;
        }
        private bool HasAnyMove()
        {
            for (int slot = 0; slot < 3; slot++)
            {
                if (slotUsed[slot])
                {
                    continue;
                }

                for (int row = 0; row < BoardSize; row++)
                {
                    for (int col = 0; col < BoardSize; col++)
                    {
                        if (CanPlace(slot, row, col))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
        private int CheckAndClearLines()
        {
            var fullRows = new bool[BoardSize];
            var fullCols = new bool[BoardSize];

            for (int row = 0; row < BoardSize; row++)
            {
                bool full = true;
                for (int col = 0; col < BoardSize; col++)
                {
                    if (boardData[row, col] == 0)
                    {
                        full = false;
                        break;
                    }
                }

                fullRows[row] = full;
            }

            for (int col = 0; col < BoardSize; col++)
            {
                bool full = true;
                for (int row = 0; row < BoardSize; row++)
                {
                    if (boardData[row, col] == 0)
                    {
                        full = false;
                        break;
                    }
                }

                fullCols[col] = full;
            }

            int clearedLines = 0;

            for (int row = 0; row < BoardSize; row++)
            {
                if (!fullRows[row])
                {
                    continue;
                }

                clearedLines++;
                for (int col = 0; col < BoardSize; col++)
                {
                    ClearCell(row, col);
                }
            }

            for (int col = 0; col < BoardSize; col++)
            {
                if (!fullCols[col])
                {
                    continue;
                }

                clearedLines++;
                for (int row = 0; row < BoardSize; row++)
                {
                    ClearCell(row, col);
                }
            }

            score += clearedLines * 10;
            return clearedLines;
        }

        private void ClearCell(int row, int col)
        {
            boardData[row, col] = 0;
            boardButtons[row, col].Background = EmptyBrush;
        }
        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            score = 0;
            gameOver = false;
            selectedSlot = -1;
            CreateBoard();
            GenerateNewBlocks();
            UpdateScore();
            txtMessage.Text = "Ván mới. Chọn một khối rồi bấm lên bàn cờ.";
        }
    }
}