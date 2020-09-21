// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package schema

type Details struct {
	ID              int           `json:"id" bson:"_id"`
	Title           string        `json:"title"`
	BackgroundImage string        `json:"backgroundImage"`
	CdKey           string        `json:"cdKey"`
	TextInformation string        `json:"textInformation"`
	Downloads       []interface{} `json:"downloads"`
	GalaxyDownloads []interface{} `json:"galaxyDownloads"`
	Extras          []struct {
		ManualURL string `json:"manualUrl"`
		Name      string `json:"name"`
		Type      string `json:"type"`
		Info      int    `json:"info"`
		Size      string `json:"size"`
	} `json:"extras"`
	DLCs []struct {
		Title           string          `json:"title"`
		BackgroundImage string          `json:"backgroundImage"`
		CdKey           string          `json:"cdKey"`
		TextInformation string          `json:"textInformation"`
		Downloads       [][]interface{} `json:"downloads"`
		Extras          []struct {
			ManualURL string `json:"manualUrl"`
			Name      string `json:"name"`
			Type      string `json:"type"`
			Info      int    `json:"info"`
			Size      string `json:"size"`
		} `json:"extras"`
		Tags             []interface{} `json:"tags"`
		IsPreOrder       bool          `json:"isPreOrder"`
		ReleaseTimestamp int           `json:"releaseTimestamp"`
		Messages         []interface{} `json:"messages"`
		Changelog        string        `json:"changelog"`
		ForumLink        string        `json:"forumLink"`
		Features         []interface{} `json:"features"`
	} `json:"dlcs"`
	Tags []struct {
		ID           string `json:"id"`
		Name         string `json:"name"`
		ProductCount string `json:"productCount"`
	} `json:"tags"`
	IsPreOrder       bool          `json:"isPreOrder"`
	ReleaseTimestamp int           `json:"releaseTimestamp"`
	Messages         []string      `json:"messages"`
	Changelog        string        `json:"changelog"`
	ForumLink        string        `json:"forumLink"`
	Features         []interface{} `json:"features"`
}

// NOTE: Not marshalling the following properties:
//
// SimpleGalaxyInstallers []struct {
// 	Path string `json:"path"`
// 	Os   string `json:"os"`
// } `json:"simpleGalaxyInstallers"`
// MissingBaseProduct     interface{}   `json:"missingBaseProduct"`
// IsBaseProductMissing   bool          `json:"isBaseProductMissing"`
