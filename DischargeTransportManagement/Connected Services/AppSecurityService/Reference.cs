﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DischargeTransportManagement.AppSecurityService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ApplicationDTO", Namespace="http://schemas.datacontract.org/2004/07/ApplicationSecurityDTO")]
    [System.SerializableAttribute()]
    public partial class ApplicationDTO : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ApplicationNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CreatedByField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.DateTime CreatedOnField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DesctiptionField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool IsActiveField;
        
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
        public string ApplicationName {
            get {
                return this.ApplicationNameField;
            }
            set {
                if ((object.ReferenceEquals(this.ApplicationNameField, value) != true)) {
                    this.ApplicationNameField = value;
                    this.RaisePropertyChanged("ApplicationName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string CreatedBy {
            get {
                return this.CreatedByField;
            }
            set {
                if ((object.ReferenceEquals(this.CreatedByField, value) != true)) {
                    this.CreatedByField = value;
                    this.RaisePropertyChanged("CreatedBy");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime CreatedOn {
            get {
                return this.CreatedOnField;
            }
            set {
                if ((this.CreatedOnField.Equals(value) != true)) {
                    this.CreatedOnField = value;
                    this.RaisePropertyChanged("CreatedOn");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Desctiption {
            get {
                return this.DesctiptionField;
            }
            set {
                if ((object.ReferenceEquals(this.DesctiptionField, value) != true)) {
                    this.DesctiptionField = value;
                    this.RaisePropertyChanged("Desctiption");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsActive {
            get {
                return this.IsActiveField;
            }
            set {
                if ((this.IsActiveField.Equals(value) != true)) {
                    this.IsActiveField = value;
                    this.RaisePropertyChanged("IsActive");
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
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="UserDTO", Namespace="http://schemas.datacontract.org/2004/07/ApplicationSecurityDTO")]
    [System.SerializableAttribute()]
    public partial class UserDTO : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ApplicationNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CreatedByField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.DateTime CreatedOnField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string FirstNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool IsActiveField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string LastNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool ShowField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string UpdatedByField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Nullable<System.DateTime> UpdatedOnField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string VunetIdField;
        
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
        public string ApplicationName {
            get {
                return this.ApplicationNameField;
            }
            set {
                if ((object.ReferenceEquals(this.ApplicationNameField, value) != true)) {
                    this.ApplicationNameField = value;
                    this.RaisePropertyChanged("ApplicationName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string CreatedBy {
            get {
                return this.CreatedByField;
            }
            set {
                if ((object.ReferenceEquals(this.CreatedByField, value) != true)) {
                    this.CreatedByField = value;
                    this.RaisePropertyChanged("CreatedBy");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime CreatedOn {
            get {
                return this.CreatedOnField;
            }
            set {
                if ((this.CreatedOnField.Equals(value) != true)) {
                    this.CreatedOnField = value;
                    this.RaisePropertyChanged("CreatedOn");
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
        public bool IsActive {
            get {
                return this.IsActiveField;
            }
            set {
                if ((this.IsActiveField.Equals(value) != true)) {
                    this.IsActiveField = value;
                    this.RaisePropertyChanged("IsActive");
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
        public bool Show {
            get {
                return this.ShowField;
            }
            set {
                if ((this.ShowField.Equals(value) != true)) {
                    this.ShowField = value;
                    this.RaisePropertyChanged("Show");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string UpdatedBy {
            get {
                return this.UpdatedByField;
            }
            set {
                if ((object.ReferenceEquals(this.UpdatedByField, value) != true)) {
                    this.UpdatedByField = value;
                    this.RaisePropertyChanged("UpdatedBy");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Nullable<System.DateTime> UpdatedOn {
            get {
                return this.UpdatedOnField;
            }
            set {
                if ((this.UpdatedOnField.Equals(value) != true)) {
                    this.UpdatedOnField = value;
                    this.RaisePropertyChanged("UpdatedOn");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string VunetId {
            get {
                return this.VunetIdField;
            }
            set {
                if ((object.ReferenceEquals(this.VunetIdField, value) != true)) {
                    this.VunetIdField = value;
                    this.RaisePropertyChanged("VunetId");
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
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="RolesDTO", Namespace="http://schemas.datacontract.org/2004/07/ApplicationSecurityDTO")]
    [System.SerializableAttribute()]
    public partial class RolesDTO : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ApplicationNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CreatedByField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.DateTime CreatedOnField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool IsActiveField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string RoleDescriptionField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string RoleNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool ShowField;
        
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
        public string ApplicationName {
            get {
                return this.ApplicationNameField;
            }
            set {
                if ((object.ReferenceEquals(this.ApplicationNameField, value) != true)) {
                    this.ApplicationNameField = value;
                    this.RaisePropertyChanged("ApplicationName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string CreatedBy {
            get {
                return this.CreatedByField;
            }
            set {
                if ((object.ReferenceEquals(this.CreatedByField, value) != true)) {
                    this.CreatedByField = value;
                    this.RaisePropertyChanged("CreatedBy");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime CreatedOn {
            get {
                return this.CreatedOnField;
            }
            set {
                if ((this.CreatedOnField.Equals(value) != true)) {
                    this.CreatedOnField = value;
                    this.RaisePropertyChanged("CreatedOn");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsActive {
            get {
                return this.IsActiveField;
            }
            set {
                if ((this.IsActiveField.Equals(value) != true)) {
                    this.IsActiveField = value;
                    this.RaisePropertyChanged("IsActive");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string RoleDescription {
            get {
                return this.RoleDescriptionField;
            }
            set {
                if ((object.ReferenceEquals(this.RoleDescriptionField, value) != true)) {
                    this.RoleDescriptionField = value;
                    this.RaisePropertyChanged("RoleDescription");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string RoleName {
            get {
                return this.RoleNameField;
            }
            set {
                if ((object.ReferenceEquals(this.RoleNameField, value) != true)) {
                    this.RoleNameField = value;
                    this.RaisePropertyChanged("RoleName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool Show {
            get {
                return this.ShowField;
            }
            set {
                if ((this.ShowField.Equals(value) != true)) {
                    this.ShowField = value;
                    this.RaisePropertyChanged("Show");
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
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="AppUserRoleDTO", Namespace="http://schemas.datacontract.org/2004/07/ApplicationSecurityDTO")]
    [System.SerializableAttribute()]
    public partial class AppUserRoleDTO : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ApplicationNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string FirstNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool IsUserActiveField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string LastNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string RoleNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool ShowUserField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string UpdatedByField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Nullable<System.DateTime> UpdatedOnField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string VunetIdField;
        
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
        public string ApplicationName {
            get {
                return this.ApplicationNameField;
            }
            set {
                if ((object.ReferenceEquals(this.ApplicationNameField, value) != true)) {
                    this.ApplicationNameField = value;
                    this.RaisePropertyChanged("ApplicationName");
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
        public bool IsUserActive {
            get {
                return this.IsUserActiveField;
            }
            set {
                if ((this.IsUserActiveField.Equals(value) != true)) {
                    this.IsUserActiveField = value;
                    this.RaisePropertyChanged("IsUserActive");
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
        public string RoleName {
            get {
                return this.RoleNameField;
            }
            set {
                if ((object.ReferenceEquals(this.RoleNameField, value) != true)) {
                    this.RoleNameField = value;
                    this.RaisePropertyChanged("RoleName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool ShowUser {
            get {
                return this.ShowUserField;
            }
            set {
                if ((this.ShowUserField.Equals(value) != true)) {
                    this.ShowUserField = value;
                    this.RaisePropertyChanged("ShowUser");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string UpdatedBy {
            get {
                return this.UpdatedByField;
            }
            set {
                if ((object.ReferenceEquals(this.UpdatedByField, value) != true)) {
                    this.UpdatedByField = value;
                    this.RaisePropertyChanged("UpdatedBy");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Nullable<System.DateTime> UpdatedOn {
            get {
                return this.UpdatedOnField;
            }
            set {
                if ((this.UpdatedOnField.Equals(value) != true)) {
                    this.UpdatedOnField = value;
                    this.RaisePropertyChanged("UpdatedOn");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string VunetId {
            get {
                return this.VunetIdField;
            }
            set {
                if ((object.ReferenceEquals(this.VunetIdField, value) != true)) {
                    this.VunetIdField = value;
                    this.RaisePropertyChanged("VunetId");
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
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="AppSecurityService.IApplicationSecurity")]
    public interface IApplicationSecurity {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IApplicationSecurity/GetAllApplications", ReplyAction="http://tempuri.org/IApplicationSecurity/GetAllApplicationsResponse")]
        DischargeTransportManagement.AppSecurityService.ApplicationDTO[] GetAllApplications();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IApplicationSecurity/GetAllApplications", ReplyAction="http://tempuri.org/IApplicationSecurity/GetAllApplicationsResponse")]
        System.Threading.Tasks.Task<DischargeTransportManagement.AppSecurityService.ApplicationDTO[]> GetAllApplicationsAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IApplicationSecurity/AddApplication", ReplyAction="http://tempuri.org/IApplicationSecurity/AddApplicationResponse")]
        string AddApplication(DischargeTransportManagement.AppSecurityService.ApplicationDTO application);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IApplicationSecurity/AddApplication", ReplyAction="http://tempuri.org/IApplicationSecurity/AddApplicationResponse")]
        System.Threading.Tasks.Task<string> AddApplicationAsync(DischargeTransportManagement.AppSecurityService.ApplicationDTO application);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IApplicationSecurity/GetAllUsers", ReplyAction="http://tempuri.org/IApplicationSecurity/GetAllUsersResponse")]
        DischargeTransportManagement.AppSecurityService.UserDTO[] GetAllUsers();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IApplicationSecurity/GetAllUsers", ReplyAction="http://tempuri.org/IApplicationSecurity/GetAllUsersResponse")]
        System.Threading.Tasks.Task<DischargeTransportManagement.AppSecurityService.UserDTO[]> GetAllUsersAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IApplicationSecurity/AddUserToApplication", ReplyAction="http://tempuri.org/IApplicationSecurity/AddUserToApplicationResponse")]
        string AddUserToApplication(DischargeTransportManagement.AppSecurityService.UserDTO user);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IApplicationSecurity/AddUserToApplication", ReplyAction="http://tempuri.org/IApplicationSecurity/AddUserToApplicationResponse")]
        System.Threading.Tasks.Task<string> AddUserToApplicationAsync(DischargeTransportManagement.AppSecurityService.UserDTO user);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IApplicationSecurity/GetAllUsersInApplication", ReplyAction="http://tempuri.org/IApplicationSecurity/GetAllUsersInApplicationResponse")]
        DischargeTransportManagement.AppSecurityService.UserDTO[] GetAllUsersInApplication(string ApplicationName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IApplicationSecurity/GetAllUsersInApplication", ReplyAction="http://tempuri.org/IApplicationSecurity/GetAllUsersInApplicationResponse")]
        System.Threading.Tasks.Task<DischargeTransportManagement.AppSecurityService.UserDTO[]> GetAllUsersInApplicationAsync(string ApplicationName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IApplicationSecurity/GetAllRoles", ReplyAction="http://tempuri.org/IApplicationSecurity/GetAllRolesResponse")]
        DischargeTransportManagement.AppSecurityService.RolesDTO[] GetAllRoles();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IApplicationSecurity/GetAllRoles", ReplyAction="http://tempuri.org/IApplicationSecurity/GetAllRolesResponse")]
        System.Threading.Tasks.Task<DischargeTransportManagement.AppSecurityService.RolesDTO[]> GetAllRolesAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IApplicationSecurity/AddRoleToApplication", ReplyAction="http://tempuri.org/IApplicationSecurity/AddRoleToApplicationResponse")]
        string AddRoleToApplication(DischargeTransportManagement.AppSecurityService.RolesDTO role);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IApplicationSecurity/AddRoleToApplication", ReplyAction="http://tempuri.org/IApplicationSecurity/AddRoleToApplicationResponse")]
        System.Threading.Tasks.Task<string> AddRoleToApplicationAsync(DischargeTransportManagement.AppSecurityService.RolesDTO role);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IApplicationSecurity/GetAllRolesInApp", ReplyAction="http://tempuri.org/IApplicationSecurity/GetAllRolesInAppResponse")]
        string[] GetAllRolesInApp(string ApplicationName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IApplicationSecurity/GetAllRolesInApp", ReplyAction="http://tempuri.org/IApplicationSecurity/GetAllRolesInAppResponse")]
        System.Threading.Tasks.Task<string[]> GetAllRolesInAppAsync(string ApplicationName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IApplicationSecurity/GetAllUsersInAppRole", ReplyAction="http://tempuri.org/IApplicationSecurity/GetAllUsersInAppRoleResponse")]
        DischargeTransportManagement.AppSecurityService.UserDTO[] GetAllUsersInAppRole(string ApplicationName, string RoleName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IApplicationSecurity/GetAllUsersInAppRole", ReplyAction="http://tempuri.org/IApplicationSecurity/GetAllUsersInAppRoleResponse")]
        System.Threading.Tasks.Task<DischargeTransportManagement.AppSecurityService.UserDTO[]> GetAllUsersInAppRoleAsync(string ApplicationName, string RoleName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IApplicationSecurity/AddUserToAppRole", ReplyAction="http://tempuri.org/IApplicationSecurity/AddUserToAppRoleResponse")]
        string AddUserToAppRole(string VunetId, string ApplicationName, string RoleName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IApplicationSecurity/AddUserToAppRole", ReplyAction="http://tempuri.org/IApplicationSecurity/AddUserToAppRoleResponse")]
        System.Threading.Tasks.Task<string> AddUserToAppRoleAsync(string VunetId, string ApplicationName, string RoleName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IApplicationSecurity/GetUserRolesInApp", ReplyAction="http://tempuri.org/IApplicationSecurity/GetUserRolesInAppResponse")]
        string[] GetUserRolesInApp(string ApplicationName, string UserName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IApplicationSecurity/GetUserRolesInApp", ReplyAction="http://tempuri.org/IApplicationSecurity/GetUserRolesInAppResponse")]
        System.Threading.Tasks.Task<string[]> GetUserRolesInAppAsync(string ApplicationName, string UserName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IApplicationSecurity/GetAppUserWithRoles", ReplyAction="http://tempuri.org/IApplicationSecurity/GetAppUserWithRolesResponse")]
        DischargeTransportManagement.AppSecurityService.AppUserRoleDTO[] GetAppUserWithRoles(string ApplicationName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IApplicationSecurity/GetAppUserWithRoles", ReplyAction="http://tempuri.org/IApplicationSecurity/GetAppUserWithRolesResponse")]
        System.Threading.Tasks.Task<DischargeTransportManagement.AppSecurityService.AppUserRoleDTO[]> GetAppUserWithRolesAsync(string ApplicationName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IApplicationSecurity/UpdateAppUserRole", ReplyAction="http://tempuri.org/IApplicationSecurity/UpdateAppUserRoleResponse")]
        void UpdateAppUserRole(DischargeTransportManagement.AppSecurityService.AppUserRoleDTO AppUserRole);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IApplicationSecurity/UpdateAppUserRole", ReplyAction="http://tempuri.org/IApplicationSecurity/UpdateAppUserRoleResponse")]
        System.Threading.Tasks.Task UpdateAppUserRoleAsync(DischargeTransportManagement.AppSecurityService.AppUserRoleDTO AppUserRole);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IApplicationSecurityChannel : DischargeTransportManagement.AppSecurityService.IApplicationSecurity, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ApplicationSecurityClient : System.ServiceModel.ClientBase<DischargeTransportManagement.AppSecurityService.IApplicationSecurity>, DischargeTransportManagement.AppSecurityService.IApplicationSecurity {
        
        public ApplicationSecurityClient() {
        }
        
        public ApplicationSecurityClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ApplicationSecurityClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ApplicationSecurityClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ApplicationSecurityClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public DischargeTransportManagement.AppSecurityService.ApplicationDTO[] GetAllApplications() {
            return base.Channel.GetAllApplications();
        }
        
        public System.Threading.Tasks.Task<DischargeTransportManagement.AppSecurityService.ApplicationDTO[]> GetAllApplicationsAsync() {
            return base.Channel.GetAllApplicationsAsync();
        }
        
        public string AddApplication(DischargeTransportManagement.AppSecurityService.ApplicationDTO application) {
            return base.Channel.AddApplication(application);
        }
        
        public System.Threading.Tasks.Task<string> AddApplicationAsync(DischargeTransportManagement.AppSecurityService.ApplicationDTO application) {
            return base.Channel.AddApplicationAsync(application);
        }
        
        public DischargeTransportManagement.AppSecurityService.UserDTO[] GetAllUsers() {
            return base.Channel.GetAllUsers();
        }
        
        public System.Threading.Tasks.Task<DischargeTransportManagement.AppSecurityService.UserDTO[]> GetAllUsersAsync() {
            return base.Channel.GetAllUsersAsync();
        }
        
        public string AddUserToApplication(DischargeTransportManagement.AppSecurityService.UserDTO user) {
            return base.Channel.AddUserToApplication(user);
        }
        
        public System.Threading.Tasks.Task<string> AddUserToApplicationAsync(DischargeTransportManagement.AppSecurityService.UserDTO user) {
            return base.Channel.AddUserToApplicationAsync(user);
        }
        
        public DischargeTransportManagement.AppSecurityService.UserDTO[] GetAllUsersInApplication(string ApplicationName) {
            return base.Channel.GetAllUsersInApplication(ApplicationName);
        }
        
        public System.Threading.Tasks.Task<DischargeTransportManagement.AppSecurityService.UserDTO[]> GetAllUsersInApplicationAsync(string ApplicationName) {
            return base.Channel.GetAllUsersInApplicationAsync(ApplicationName);
        }
        
        public DischargeTransportManagement.AppSecurityService.RolesDTO[] GetAllRoles() {
            return base.Channel.GetAllRoles();
        }
        
        public System.Threading.Tasks.Task<DischargeTransportManagement.AppSecurityService.RolesDTO[]> GetAllRolesAsync() {
            return base.Channel.GetAllRolesAsync();
        }
        
        public string AddRoleToApplication(DischargeTransportManagement.AppSecurityService.RolesDTO role) {
            return base.Channel.AddRoleToApplication(role);
        }
        
        public System.Threading.Tasks.Task<string> AddRoleToApplicationAsync(DischargeTransportManagement.AppSecurityService.RolesDTO role) {
            return base.Channel.AddRoleToApplicationAsync(role);
        }
        
        public string[] GetAllRolesInApp(string ApplicationName) {
            return base.Channel.GetAllRolesInApp(ApplicationName);
        }
        
        public System.Threading.Tasks.Task<string[]> GetAllRolesInAppAsync(string ApplicationName) {
            return base.Channel.GetAllRolesInAppAsync(ApplicationName);
        }
        
        public DischargeTransportManagement.AppSecurityService.UserDTO[] GetAllUsersInAppRole(string ApplicationName, string RoleName) {
            return base.Channel.GetAllUsersInAppRole(ApplicationName, RoleName);
        }
        
        public System.Threading.Tasks.Task<DischargeTransportManagement.AppSecurityService.UserDTO[]> GetAllUsersInAppRoleAsync(string ApplicationName, string RoleName) {
            return base.Channel.GetAllUsersInAppRoleAsync(ApplicationName, RoleName);
        }
        
        public string AddUserToAppRole(string VunetId, string ApplicationName, string RoleName) {
            return base.Channel.AddUserToAppRole(VunetId, ApplicationName, RoleName);
        }
        
        public System.Threading.Tasks.Task<string> AddUserToAppRoleAsync(string VunetId, string ApplicationName, string RoleName) {
            return base.Channel.AddUserToAppRoleAsync(VunetId, ApplicationName, RoleName);
        }
        
        public string[] GetUserRolesInApp(string ApplicationName, string UserName) {
            return base.Channel.GetUserRolesInApp(ApplicationName, UserName);
        }
        
        public System.Threading.Tasks.Task<string[]> GetUserRolesInAppAsync(string ApplicationName, string UserName) {
            return base.Channel.GetUserRolesInAppAsync(ApplicationName, UserName);
        }
        
        public DischargeTransportManagement.AppSecurityService.AppUserRoleDTO[] GetAppUserWithRoles(string ApplicationName) {
            return base.Channel.GetAppUserWithRoles(ApplicationName);
        }
        
        public System.Threading.Tasks.Task<DischargeTransportManagement.AppSecurityService.AppUserRoleDTO[]> GetAppUserWithRolesAsync(string ApplicationName) {
            return base.Channel.GetAppUserWithRolesAsync(ApplicationName);
        }
        
        public void UpdateAppUserRole(DischargeTransportManagement.AppSecurityService.AppUserRoleDTO AppUserRole) {
            base.Channel.UpdateAppUserRole(AppUserRole);
        }
        
        public System.Threading.Tasks.Task UpdateAppUserRoleAsync(DischargeTransportManagement.AppSecurityService.AppUserRoleDTO AppUserRole) {
            return base.Channel.UpdateAppUserRoleAsync(AppUserRole);
        }
    }
}
