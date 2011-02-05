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

        free(Option[0].Value.pszValue);
    }

    void ProxyConfigurator::SetProxy(String ^address)
    {
        INTERNET_PER_CONN_OPTION_LIST    List;
        INTERNET_PER_CONN_OPTION         Option[2]; 
        unsigned long                    nSize = sizeof(INTERNET_PER_CONN_OPTION_LIST); 
        const pin_ptr<const wchar_t> ppchar = PtrToStringChars(address);

        Option[0].dwOption = INTERNET_PER_CONN_PROXY_SERVER;
        Option[0].Value.pszValue = _wcsdup(ppchar);

        Option[1].dwOption = INTERNET_PER_CONN_FLAGS;
        Option[1].Value.dwValue = PROXY_TYPE_PROXY;
        Option[1].Value.dwValue |= PROXY_TYPE_DIRECT;

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

        free(Option[0].Value.pszValue);
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

        // Tell IE the settings changed
        InternetSetOption(NULL, INTERNET_OPTION_SETTINGS_CHANGED, NULL, 0);
        InternetSetOption(NULL, INTERNET_OPTION_REFRESH , NULL, 0);
    }
}