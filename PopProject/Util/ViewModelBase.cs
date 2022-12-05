using System.ComponentModel;

public abstract class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
 
    protected  void onPropertyChanged(string propName)
    {
        PropertyChangedEventHandler handler = PropertyChanged; 
        if (handler != null) { 
            handler(this, new PropertyChangedEventArgs(propName));
        }
      
    }
}
