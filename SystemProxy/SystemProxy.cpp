// Il s'agit du fichier DLL principal.

#include "stdafx.h"

#include "SystemProxy.h"

namespace SystemProxy {

	void ProxyConfigurator::SetProxyPac(String ^pacFile)
	{
		log->Info("SetProxyPac pacFile: " + pacFile);

		INTERNET_PER_CONN_OPTION_LIST    list;
		INTERNET_PER_CONN_OPTION         options[2]; 
		unsigned long                    nSize = sizeof(INTERNET_PER_CONN_OPTION_LIST); 
		const pin_ptr<const wchar_t> ppchar = PtrToStringChars(pacFile);

		options[0].dwOption = INTERNET_PER_CONN_AUTOCONFIG_URL; 
		options[0].Value.pszValue = _wcsdup(ppchar);

		options[1].dwOption = INTERNET_PER_CONN_FLAGS; 
		options[1].Value.dwValue = PROXY_TYPE_AUTO_PROXY_URL; 

		list.dwSize = sizeof(INTERNET_PER_CONN_OPTION_LIST); 
		list.pszConnection = NULL;
		list.dwOptionCount = sizeof(options)/sizeof(*options); 
		list.dwOptionError = 0;
		list.pOptions = options;

		if(!InternetSetOption(NULL, INTERNET_OPTION_PER_CONNECTION_OPTION, &list, nSize)) 
		{
			free(options[0].Value.pszValue);
			log->Error("InternetSetOption INTERNET_OPTION_PER_CONNECTION_OPTION failed");
			throw gcnew Exception("InternetSetOption: " + GetLastError());
		}

		Refresh();

		free(options[0].Value.pszValue);
	}

	void ProxyConfigurator::SetProxy(String ^address, String ^exceptions)
	{
		log->Info("SetProxy address: " + address + ", exceptions: " + exceptions);

		INTERNET_PER_CONN_OPTION_LIST    list;
		INTERNET_PER_CONN_OPTION         options[3];
		unsigned long                    nSize = sizeof(INTERNET_PER_CONN_OPTION_LIST); 
		pin_ptr<const wchar_t> ppAddress = PtrToStringChars(address);
		pin_ptr<const wchar_t> ppExceptions = nullptr;

		int optionsId = 0;

		options[optionsId].dwOption = INTERNET_PER_CONN_PROXY_SERVER;
		options[optionsId].Value.pszValue = (wchar_t*)ppAddress;
		optionsId++;

		options[optionsId].dwOption = INTERNET_PER_CONN_FLAGS;
		options[optionsId].Value.dwValue = PROXY_TYPE_PROXY | PROXY_TYPE_DIRECT;
		optionsId++;

		if (exceptions != nullptr)
		{
			ppExceptions = PtrToStringChars(exceptions);
			options[optionsId].dwOption = INTERNET_PER_CONN_PROXY_BYPASS;
			options[optionsId].Value.pszValue = (wchar_t*)ppExceptions;
			optionsId++;
		}

		list.dwSize = sizeof(INTERNET_PER_CONN_OPTION_LIST);
		list.pszConnection = NULL;
		list.dwOptionCount = optionsId; 
		list.dwOptionError = 0;
		list.pOptions = options;

		if(!InternetSetOption(NULL, INTERNET_OPTION_PER_CONNECTION_OPTION, &list, nSize)) 
		{
			log->Error("InternetSetOption INTERNET_OPTION_PER_CONNECTION_OPTION failed");
			throw gcnew Exception("InternetSetOption: " + GetLastError());
		}

		Refresh();
	}

	void ProxyConfigurator::ResetProxy()
	{
		log->Info("ResetProxy");

		//conn_name: active connection name. 
		INTERNET_PER_CONN_OPTION_LIST list;
		INTERNET_PER_CONN_OPTION options[1];

		DWORD   dwBufSize = sizeof(list);

		list.dwSize = sizeof(list);
		list.pszConnection = NULL;
		list.dwOptionCount = sizeof(options)/sizeof(*options);
		list.pOptions = options;

		// Set flags.
		list.pOptions[0].dwOption = INTERNET_PER_CONN_FLAGS;
		list.pOptions[0].Value.dwValue = PROXY_TYPE_DIRECT ;

		if (!InternetSetOption(NULL, INTERNET_OPTION_PER_CONNECTION_OPTION, &list, dwBufSize))
		{
			log->Error("InternetSetOption INTERNET_OPTION_PER_CONNECTION_OPTION failed");
			throw gcnew Exception("InternetSetOption: " + GetLastError());
		}

		Refresh();
	}

	// Tell IE the settings changed
	void ProxyConfigurator::Refresh()
	{
		if (!::InternetSetOption(NULL, INTERNET_OPTION_PROXY_SETTINGS_CHANGED, NULL, 0))
		{
			log->Error("InternetSetOption INTERNET_OPTION_PROXY_SETTINGS_CHANGED failed");
		}

		/*
		if (!::InternetSetOption(NULL, INTERNET_OPTION_SETTINGS_CHANGED, NULL, 0))
		{
			log->Error("InternetSetOption INTERNET_OPTION_SETTINGS_CHANGED failed");
		}
		*/

		if (!::InternetSetOption(NULL, INTERNET_OPTION_REFRESH , NULL, 0))
		{
			log->Error("InternetSetOption INTERNET_OPTION_REFRESH failed");
		}
	}
}