using System;
using LinqToRdf;
using System.Data.Linq;
using System.ComponentModel;

namespace SystemScannerModel 
{

[OwlResource(OntologyName="$fileinputname$", RelativeUriReference="Artifact")]
public partial class Artifact : OwlInstanceSupertype, INotifyPropertyChanging, INotifyPropertyChanged
{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		partial void OnArtifactExistsChanging(bool value);
		partial void OnArtifactExistsChanged(bool value);
		private bool _ArtifactExists;
		[OwlResource(OntologyName = "SystemScannerModel ", RelativeUriReference="artifactExists")]
		public bool ArtifactExists
		{
			get
			{
				return this._ArtifactExists;
			}
			set
			{
				if ((this._ArtifactExists != value))
				{
					this.OnArtifactExistsChanging(value);
					this.SendPropertyChanging();
					this._ArtifactExists = value;
					this.SendPropertyChanged("ArtifactExists");
					this.OnArtifactExistsChanged(value);
				}
			}
		}
		partial void OnDateCreatedChanging(DateTime value);
		partial void OnDateCreatedChanged(DateTime value);
		private DateTime _DateCreated;
		[OwlResource(OntologyName = "SystemScannerModel ", RelativeUriReference="dateCreated")]
		public DateTime DateCreated
		{
			get
			{
				return this._DateCreated;
			}
			set
			{
				if ((this._DateCreated != value))
				{
					this.OnDateCreatedChanging(value);
					this.SendPropertyChanging();
					this._DateCreated = value;
					this.SendPropertyChanged("DateCreated");
					this.OnDateCreatedChanged(value);
				}
			}
		}
		partial void OnDateLastModifiedChanging(DateTime value);
		partial void OnDateLastModifiedChanged(DateTime value);
		private DateTime _DateLastModified;
		[OwlResource(OntologyName = "SystemScannerModel ", RelativeUriReference="dateLastModified")]
		public DateTime DateLastModified
		{
			get
			{
				return this._DateLastModified;
			}
			set
			{
				if ((this._DateLastModified != value))
				{
					this.OnDateLastModifiedChanging(value);
					this.SendPropertyChanging();
					this._DateLastModified = value;
					this.SendPropertyChanged("DateLastModified");
					this.OnDateLastModifiedChanged(value);
				}
			}
		}
		partial void OnFilePathChanging(string value);
		partial void OnFilePathChanged(string value);
		private string _FilePath;
		[OwlResource(OntologyName = "SystemScannerModel ", RelativeUriReference="filePath")]
		public string FilePath
		{
			get
			{
				return this._FilePath;
			}
			set
			{
				if ((this._FilePath != value))
				{
					this.OnFilePathChanging(value);
					this.SendPropertyChanging();
					this._FilePath = value;
					this.SendPropertyChanged("FilePath");
					this.OnFilePathChanged(value);
				}
			}
		}
		partial void OnIsReadOnlyChanging(bool value);
		partial void OnIsReadOnlyChanged(bool value);
		private bool _IsReadOnly;
		[OwlResource(OntologyName = "SystemScannerModel ", RelativeUriReference="isReadOnly")]
		public bool IsReadOnly
		{
			get
			{
				return this._IsReadOnly;
			}
			set
			{
				if ((this._IsReadOnly != value))
				{
					this.OnIsReadOnlyChanging(value);
					this.SendPropertyChanging();
					this._IsReadOnly = value;
					this.SendPropertyChanged("IsReadOnly");
					this.OnIsReadOnlyChanged(value);
				}
			}
		}
		partial void OnHasACLChanging(string value);
		partial void OnHasACLChanged(string value);
		private string _HasACL;
		[OwlResource(OntologyName = "SystemScannerModel ", RelativeUriReference="hasACL")]
		public string HasACL
		{
			get
			{
				return this._HasACL;
			}
			set
			{
				if ((this._HasACL != value))
				{
					this.OnHasACLChanging(value);
					this.SendPropertyChanging();
					this._HasACL = value;
					this.SendPropertyChanged("HasACL");
					this.OnHasACLChanged(value);
				}
			}
		}
		partial void OnHasDependencyOnChanging(Artifact value);
		partial void OnHasDependencyOnChanged(Artifact value);
		private EntitySet<Artifact> _HasDependencyOn;
		[OwlResource(OntologyName = "SystemScannerModel ", RelativeUriReference="hasDependencyOn")]
		public EntitySet<Artifact> HasDependencyOn
		{
			get
			{
				return this._HasDependencyOn;
			}
			set
			{
				if ((this._HasDependencyOn != value))
				{
					this.OnHasDependencyOnChanging(value);
					this.SendPropertyChanging();
					this._HasDependencyOn.Assign(value);
					this.SendPropertyChanged("HasDependencyOn");
					this.OnHasDependencyOnChanged();
				}
			}
		}

		public event PropertyChangingEventHandler PropertyChanging;
		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate();
    partial void OnCreated();
	#endregion
}

[OwlResource(OntologyName="$fileinputname$", RelativeUriReference="assembly")]
public partial class Assembly : Artifact, INotifyPropertyChanging, INotifyPropertyChanged
{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		partial void OnIsSignedChanging(bool value);
		partial void OnIsSignedChanged(bool value);
		private bool _IsSigned;
		[OwlResource(OntologyName = "SystemScannerModel ", RelativeUriReference="isSigned")]
		public bool IsSigned
		{
			get
			{
				return this._IsSigned;
			}
			set
			{
				if ((this._IsSigned != value))
				{
					this.OnIsSignedChanging(value);
					this.SendPropertyChanging();
					this._IsSigned = value;
					this.SendPropertyChanged("IsSigned");
					this.OnIsSignedChanged(value);
				}
			}
		}
		partial void OnStrongKeyChanging(string value);
		partial void OnStrongKeyChanged(string value);
		private string _StrongKey;
		[OwlResource(OntologyName = "SystemScannerModel ", RelativeUriReference="strongKey")]
		public string StrongKey
		{
			get
			{
				return this._StrongKey;
			}
			set
			{
				if ((this._StrongKey != value))
				{
					this.OnStrongKeyChanging(value);
					this.SendPropertyChanging();
					this._StrongKey = value;
					this.SendPropertyChanged("StrongKey");
					this.OnStrongKeyChanged(value);
				}
			}
		}
		partial void OnVersionChanging(string value);
		partial void OnVersionChanged(string value);
		private string _Version;
		[OwlResource(OntologyName = "SystemScannerModel ", RelativeUriReference="version")]
		public string Version
		{
			get
			{
				return this._Version;
			}
			set
			{
				if ((this._Version != value))
				{
					this.OnVersionChanging(value);
					this.SendPropertyChanging();
					this._Version = value;
					this.SendPropertyChanged("Version");
					this.OnVersionChanged(value);
				}
			}
		}

		public event PropertyChangingEventHandler PropertyChanging;
		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate();
    partial void OnCreated();
	#endregion
}
}