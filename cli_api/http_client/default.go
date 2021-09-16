package http_client

import (
	"github.com/boggydigital/vangogh/cli_api/cookies"
	"net"
	"net/http"
	"time"
)

func Default() (*http.Client, error) {
	jar, err := cookies.LoadJar()
	if err != nil {
		return nil, err
	}

	return &http.Client{
		Transport: &http.Transport{
			DialContext: (&net.Dialer{
				Timeout:   20 * time.Second,
				KeepAlive: 20 * time.Second,
			}).DialContext,
			TLSHandshakeTimeout:   20 * time.Second,
			ExpectContinueTimeout: 20 * time.Second,
			ResponseHeaderTimeout: 20 * time.Second,
			IdleConnTimeout:       20 * time.Second,
		},
		Jar: jar,
	}, nil
}
