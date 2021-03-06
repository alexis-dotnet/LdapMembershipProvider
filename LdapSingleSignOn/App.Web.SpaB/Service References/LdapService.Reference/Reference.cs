﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace App.Web.SpaB.LdapService.Reference {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="LdapUser", Namespace="http://schemas.datacontract.org/2004/07/App.Service")]
    [System.SerializableAttribute()]
    public partial class LdapUser : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string EmailField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string FaxField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string FirstNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string FullNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int IdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool IsOnlineField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.DateTime LastActivityDateField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.DateTime LastLoginDateField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string LastNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string LocationField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string PhoneField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string[] RolesField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int RoomNumberField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string UsernameField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Email {
            get {
                return this.EmailField;
            }
            set {
                if ((object.ReferenceEquals(this.EmailField, value) != true)) {
                    this.EmailField = value;
                    this.RaisePropertyChanged("Email");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Fax {
            get {
                return this.FaxField;
            }
            set {
                if ((object.ReferenceEquals(this.FaxField, value) != true)) {
                    this.FaxField = value;
                    this.RaisePropertyChanged("Fax");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string FirstName {
            get {
                return this.FirstNameField;
            }
            set {
                if ((object.ReferenceEquals(this.FirstNameField, value) != true)) {
                    this.FirstNameField = value;
                    this.RaisePropertyChanged("FirstName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string FullName {
            get {
                return this.FullNameField;
            }
            set {
                if ((object.ReferenceEquals(this.FullNameField, value) != true)) {
                    this.FullNameField = value;
                    this.RaisePropertyChanged("FullName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Id {
            get {
                return this.IdField;
            }
            set {
                if ((this.IdField.Equals(value) != true)) {
                    this.IdField = value;
                    this.RaisePropertyChanged("Id");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsOnline {
            get {
                return this.IsOnlineField;
            }
            set {
                if ((this.IsOnlineField.Equals(value) != true)) {
                    this.IsOnlineField = value;
                    this.RaisePropertyChanged("IsOnline");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime LastActivityDate {
            get {
                return this.LastActivityDateField;
            }
            set {
                if ((this.LastActivityDateField.Equals(value) != true)) {
                    this.LastActivityDateField = value;
                    this.RaisePropertyChanged("LastActivityDate");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime LastLoginDate {
            get {
                return this.LastLoginDateField;
            }
            set {
                if ((this.LastLoginDateField.Equals(value) != true)) {
                    this.LastLoginDateField = value;
                    this.RaisePropertyChanged("LastLoginDate");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string LastName {
            get {
                return this.LastNameField;
            }
            set {
                if ((object.ReferenceEquals(this.LastNameField, value) != true)) {
                    this.LastNameField = value;
                    this.RaisePropertyChanged("LastName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Location {
            get {
                return this.LocationField;
            }
            set {
                if ((object.ReferenceEquals(this.LocationField, value) != true)) {
                    this.LocationField = value;
                    this.RaisePropertyChanged("Location");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Phone {
            get {
                return this.PhoneField;
            }
            set {
                if ((object.ReferenceEquals(this.PhoneField, value) != true)) {
                    this.PhoneField = value;
                    this.RaisePropertyChanged("Phone");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string[] Roles {
            get {
                return this.RolesField;
            }
            set {
                if ((object.ReferenceEquals(this.RolesField, value) != true)) {
                    this.RolesField = value;
                    this.RaisePropertyChanged("Roles");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int RoomNumber {
            get {
                return this.RoomNumberField;
            }
            set {
                if ((this.RoomNumberField.Equals(value) != true)) {
                    this.RoomNumberField = value;
                    this.RaisePropertyChanged("RoomNumber");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Username {
            get {
                return this.UsernameField;
            }
            set {
                if ((object.ReferenceEquals(this.UsernameField, value) != true)) {
                    this.UsernameField = value;
                    this.RaisePropertyChanged("Username");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="LdapService.Reference.ILdapService")]
    public interface ILdapService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILdapService/GetUser", ReplyAction="http://tempuri.org/ILdapService/GetUserResponse")]
        App.Web.SpaB.LdapService.Reference.LdapUser GetUser(string username);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILdapService/GetUser", ReplyAction="http://tempuri.org/ILdapService/GetUserResponse")]
        System.Threading.Tasks.Task<App.Web.SpaB.LdapService.Reference.LdapUser> GetUserAsync(string username);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILdapService/GetAllUsers", ReplyAction="http://tempuri.org/ILdapService/GetAllUsersResponse")]
        App.Web.SpaB.LdapService.Reference.LdapUser[] GetAllUsers();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILdapService/GetAllUsers", ReplyAction="http://tempuri.org/ILdapService/GetAllUsersResponse")]
        System.Threading.Tasks.Task<App.Web.SpaB.LdapService.Reference.LdapUser[]> GetAllUsersAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILdapService/GetOnlineUsers", ReplyAction="http://tempuri.org/ILdapService/GetOnlineUsersResponse")]
        App.Web.SpaB.LdapService.Reference.LdapUser[] GetOnlineUsers();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILdapService/GetOnlineUsers", ReplyAction="http://tempuri.org/ILdapService/GetOnlineUsersResponse")]
        System.Threading.Tasks.Task<App.Web.SpaB.LdapService.Reference.LdapUser[]> GetOnlineUsersAsync();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ILdapServiceChannel : App.Web.SpaB.LdapService.Reference.ILdapService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class LdapServiceClient : System.ServiceModel.ClientBase<App.Web.SpaB.LdapService.Reference.ILdapService>, App.Web.SpaB.LdapService.Reference.ILdapService {
        
        public LdapServiceClient() {
        }
        
        public LdapServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public LdapServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public LdapServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public LdapServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public App.Web.SpaB.LdapService.Reference.LdapUser GetUser(string username) {
            return base.Channel.GetUser(username);
        }
        
        public System.Threading.Tasks.Task<App.Web.SpaB.LdapService.Reference.LdapUser> GetUserAsync(string username) {
            return base.Channel.GetUserAsync(username);
        }
        
        public App.Web.SpaB.LdapService.Reference.LdapUser[] GetAllUsers() {
            return base.Channel.GetAllUsers();
        }
        
        public System.Threading.Tasks.Task<App.Web.SpaB.LdapService.Reference.LdapUser[]> GetAllUsersAsync() {
            return base.Channel.GetAllUsersAsync();
        }
        
        public App.Web.SpaB.LdapService.Reference.LdapUser[] GetOnlineUsers() {
            return base.Channel.GetOnlineUsers();
        }
        
        public System.Threading.Tasks.Task<App.Web.SpaB.LdapService.Reference.LdapUser[]> GetOnlineUsersAsync() {
            return base.Channel.GetOnlineUsersAsync();
        }
    }
}
