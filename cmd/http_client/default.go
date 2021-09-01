package http_client

import (
	"github.com/boggydigital/vangogh/cmd/cookies"
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
				Timeout:   10 * time.Second,
				KeepAlive: 10 * time.Second,
			}).DialContext,
			TLSHandshakeTimeout:   10 * time.Second,
			ExpectContinueTimeout: 10 * time.Second,
			ResponseHeaderTimeout: 10 * time.Second,
			IdleConnTimeout:       10 * time.Second,
		},
		Jar: jar,
	}, nil
}