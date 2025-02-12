package cli

import (
	"github.com/boggydigital/nod"
	"io"
	"net/http"
	"net/url"
)

func HealthHandler(u *url.URL) error {
	return Health(u.Query().Get("url"))
}

func Health(u string) error {

	ha := nod.Begin("checking health...")
	defer ha.Done()

	resp, err := http.Get(u)
	if err != nil {
		panic(err)
	}
	defer resp.Body.Close()

	health, err := io.ReadAll(resp.Body)
	if err != nil {
		panic(err)
	}

	ha.EndWithResult(string(health))

	return nil
}
