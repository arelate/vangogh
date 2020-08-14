package pages

import (
	"io/ioutil"
	"net/http"
	"net/url"
	"strconv"
)

func Fetch(client *http.Client, url *url.URL, page int) (*[]byte, error) {
	q := url.Query()
	q.Add("page", strconv.Itoa(page))
	url.RawQuery = q.Encode()

	resp, _ := client.Get(url.String())
	defer resp.Body.Close()

	respBody, err := ioutil.ReadAll(resp.Body)
	return &respBody, err
}
