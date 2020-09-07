package origin

import (
	"io/ioutil"
	"net/http"
	"net/url"
)

type Source struct {
	HttpClient *http.Client
	URL        *url.URL
}

func NewSource(client *http.Client, url *url.URL) *Source {
	return &Source{
		HttpClient: client,
		URL:        url,
	}
}

func (source *Source) Get(id int) (*[]byte, error) {
	resp, _ := source.HttpClient.Get(source.URL.String())

	respBody, err := ioutil.ReadAll(resp.Body)
	if err != nil {
		return nil, err
	}

	return &respBody, resp.Body.Close()
}
