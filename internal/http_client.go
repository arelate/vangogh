package internal

import (
	"net"
	"net/http"
	"time"
)

func HttpClient() (*http.Client, error) {
	jar, err := LoadCookieJar()
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
		},
		Jar: jar,
	}, nil
}
