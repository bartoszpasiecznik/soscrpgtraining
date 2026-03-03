using System.ComponentModel;

namespace Engine.Models;

public class Player : INotifyPropertyChanged
{
    private string _name;
    private string _characterClass;
    private int _hitPoints;
    private int _experiencePoints;
    private int _level;
    private int _gold;

    public string Name
    {
        get { return _name; }
        set { _name = value; OnPropoertyChanged(nameof(Name)); }
    }

    public string CharacterClass
    {
        get  { return _characterClass; } 
        set{ _characterClass = value; OnPropoertyChanged(nameof(CharacterClass)); }
    }

    public int HitPoints
    {
        get {return _hitPoints;}
        set  { _hitPoints = value; OnPropoertyChanged(nameof(HitPoints)); }
    }

    public int ExperiencePoints
    {
        get{return _experiencePoints;}
        set{_experiencePoints = value; OnPropoertyChanged(nameof(ExperiencePoints)); }
    }

    public int Level
    {
        get{return _level;}
        set{ _level = value; OnPropoertyChanged(nameof(Level)); }
    }

    public int Gold
    {
        get{return _gold;}
        set{ _gold = value; OnPropoertyChanged(nameof(Gold)); }
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropoertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}