// Il s'agit du fichier DLL principal.

#include "stdafx.h"

#include "SystemProxy.h"

namespace SystemProxy {

	void ProxyConfigurator::SetProxyPac(String ^pacFile)
	{
		INTERNET_PER_CONN_OPTION_LIST    List;
		INTERNET_PER_CONN_OPTION         Option[2]; 
		unsigned long                    nSize = sizeof(INTERNET_PER_CONN_OPTION_LIST); 
		const pin_ptr<const wchar_t> ppchar = PtrToStringChars(pacFile);

		Option[0].dwOption = INTERNET_PER_CONN_AUTOCONFIG_URL; 
		Option[0].Value.pszValue = _wcsdup(ppchar);

		Option[1].dwOption = INTERNET_PER_CONN_FLAGS; 
		Option[1].Value.dwValue = PROXY_TYPE_AUTO_PROXY_URL; 

		List.dwSize = sizeof(INTERNET_PER_CONN_OPTION_LIST); 
		List.pszConnection = NULL;
		List.dwOptionCount = sizeof(Option)/sizeof(*Option); 
		List.dwOptionError = 0;
		List.pOptions = Option;

		if(!InternetSetOption(NULL, INTERNET_OPTION_PER_CONNECTION_OPTION, &List, nSize)) 
		{
			free(Option[0].Value.pszValue);
			throw gcnew Exception("InternetSetOption: " + GetLastError());
		}

		RefreshIE();

		free(Option[0].Value.pszValue);
	}

	void ProxyConfigurator::SetProxy(String ^address, String ^exceptions)
	{
		INTERNET_PER_CONN_OPTION_LIST    List;
		INTERNET_PER_CONN_OPTION         Option[3];
		unsigned long                    nSize = sizeof(INTERNET_PER_CONN_OPTION_LIST); 
		pin_ptr<const wchar_t> ppAddress = PtrToStringChars(address);
		pin_ptr<const wchar_t> ppExceptions = nullptr;

		int optionsId = 0;

		Option[optionsId].dwOption = INTERNET_PER_CONN_PROXY_SERVER;
		Option[optionsId].Value.pszValue = (wchar_t*)ppAddress;
		optionsId++;

		if (exceptions != nullptr)
		{
			ppExceptions = PtrToStringChars(exceptions);
			Option[optionsId].dwOption = INTERNET_PER_CONN_PROXY_BYPASS;
			Option[optionsId].Value.pszValue = (wchar_t*)ppExceptions;
			optionsId++;
		}

		Option[optionsId].dwOption = INTERNET_PER_CONN_FLAGS;
		Option[optionsId].Value.dwValue = PROXY_TYPE_PROXY;
		Option[optionsId].Value.dwValue |= PROXY_TYPE_DIRECT;
		optionsId++;

		List.dwSize = sizeof(INTERNET_PER_CONN_OPTION_LIST);;
		List.pszConnection = NULL;
		List.dwOptionCount = optionsId; 
		List.dwOptionError = 0;
		List.pOptions = Option;

		if(!InternetSetOption(NULL, INTERNET_OPTION_PER_CONNECTION_OPTION, &List, nSize)) 
		{
			throw gcnew Exception("InternetSetOption: " + GetLastError());
		}

		RefreshIE();
	}

	void ProxyConfigurator::ResetProxy()
	{
		//conn_name: active connection name. 
		INTERNET_PER_CONN_OPTION_LIST list;
		INTERNET_PER_CONN_OPTION Option[1];

		DWORD   dwBufSize = sizeof(list);

		list.dwSize = sizeof(list);
		list.pszConnection = NULL;
		list.dwOptionCount = sizeof(Option)/sizeof(*Option);
		list.pOptions = Option;

		// Set flags.
		list.pOptions[0].dwOption = INTERNET_PER_CONN_FLAGS;
		list.pOptions[0].Value.dwValue = PROXY_TYPE_DIRECT ;

		if (!InternetSetOption(NULL, INTERNET_OPTION_PER_CONNECTION_OPTION, &list, dwBufSize))
		{
			throw gcnew Exception("InternetSetOption: " + GetLastError());
		}

		RefreshIE();
	}

	// Tell IE the settings changed
	void ProxyConfigurator::RefreshIE()
	{
		InternetSetOption(NULL, INTERNET_OPTION_PROXY_SETTINGS_CHANGED, NULL, 0);
		InternetSetOption(NULL, INTERNET_OPTION_REFRESH , NULL, 0);
	}
}