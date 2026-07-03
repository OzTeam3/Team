using System.ComponentModel;

public class PlayerViewModel : INotifyPropertyChanged
{
    private string _name;
   
    public string Name
    {  
        get { return _name; } 
        set 
        {
            if (_name != value)
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
    }

    private float _hp;
    public float Hp
    {
        get { return _hp; }
        set
        {
            if (_hp != value)
            {
                _hp = value;
                OnPropertyChanged(nameof(Hp));
            }
        }
    }

    private int _level;
    public int Level
    {
        get { return _level; }
        set
        {
            if (_level != value)
            {
                _level = value;
                OnPropertyChanged(nameof(Level));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public void SyncProperty()
    {
        OnPropertyChanged(nameof(Name));
        OnPropertyChanged(nameof(Hp));
        OnPropertyChanged(nameof(Level));
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
