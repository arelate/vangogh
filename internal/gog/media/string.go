// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package media

func (mt Type) String() string {
	switch mt {
	case Unknown:
		return unknown
	case Game:
		return game
	case Movie:
		return movie
	default:
		return ""
	}
}
