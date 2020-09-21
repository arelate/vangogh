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
	"strings"
)

const idPlaceholder = "{id}"

type Details struct {
	*Source
}

func NewDetails(httpClient *http.Client, urlTemplate *url.URL) *Details {
	return &Details{
		Source: NewSource(httpClient, urlTemplate),
	}
}

func (details *Details) Get(id int) (*[]byte, error) {
	// use id
	details.URL.Path = strings.Replace(details.URL.Path, idPlaceholder, strconv.Itoa(id), 1)

	return details.Source.Get(id)
}

func (details *Details) Transfer(id int, setter local.Setter) error {
	bytes, _ := details.Get(id)

	var det map[string]interface{}
	_ = json.Unmarshal(*bytes, &det)
	det["id"] = id

	djson, _ := json.Marshal(det)
	_ = setter.Set(djson)

	return nil
}
