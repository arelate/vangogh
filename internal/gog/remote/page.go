// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package remote

import (
	"encoding/json"
	"github.com/boggydigital/vangogh/internal/gog/local"
	"net/http"
	"net/url"
	"strconv"
)

const pageKey = "page"

type Page struct {
	*Source
}

func NewPage(client *http.Client, url *url.URL) *Page {
	return &Page{
		Source: NewSource(client, url),
	}
}

func (pageSource *Page) Get(page int) (*[]byte, error) {

	q := pageSource.URL.Query()
	q.Add(pageKey, strconv.Itoa(page))
	pageSource.URL.RawQuery = q.Encode()

	return pageSource.Source.Get(page)
}

func (pageSource *Page) TransferPage(page int, setter local.Setter, itemSet func(int)) (totalPages int, err error) {

	bytes, _ := pageSource.Get(page)

	var pageData map[string]interface{}
	_ = json.Unmarshal(*bytes, &pageData)

	for i, p := range pageData["products"].([]interface{}) {
		pjson, _ := json.Marshal(p)
		_ = setter.Set(page, pjson)
		if itemSet != nil {
			itemSet(i)
		}
	}

	return int(pageData["totalPages"].(float64)), nil
}
