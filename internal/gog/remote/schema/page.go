// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package schema

type Page struct {
	Page             int    `json:"page"`
	TotalPages       int    `json:"totalPages"`
	TotalResults     string `json:"totalResults"`
	TotalGamesFound  int    `json:"totalGamesFound"`
	TotalMoviesFound int    `json:"totalMoviesFound"`
}
