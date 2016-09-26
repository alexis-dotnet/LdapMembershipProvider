Description
===========
This is a custom MembershipProvider created to work with a LDAP Server. It has a demonstration application using the provider in order to implement security for ASP.NET Web Forms Authentication.

Installation and Configuration
==============================

1. Check if you have installed Java JRE 7 in your machine, if it is not present please install it.
2. Execute LDAP Server
2.1. Open folder Source\LdapServer in explorer
2.2. Double click "start.bat". It will launch a statndalone in-memory LDAP server using the "example.ldif" file containg sample data.
2.3. Server will be running on port 1234
3. Open Visual Studio Solution
3.1. Open folder Source\LdapSingleSignOn in explorer
3.2. Double click "LdapSingleSignOn.sln". It will launch and open the entire solution in Visual Studio.
4. Compile solution
4.1. Press Ctrl+Shift+B, it will compile the entire solution
	
