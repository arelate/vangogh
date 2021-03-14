package internal

import (
	"net/http"
	"time"
)

func HttpClient() (*http.Client, error) {
	jar, err := LoadCookieJar()
	if err != nil {
		return nil, err
	}

	return &http.Client{
		Timeout: time.Second * 15,
		Jar:     jar,
	}, nil
}
