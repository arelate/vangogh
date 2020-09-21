// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package attr

import "golang.org/x/net/html"

func Val(node *html.Node, attribute string) string {
	if node == nil {
		return ""
	}
	for _, attr := range node.Attr {
		if attr.Key == attribute {
			return attr.Val
		}
	}
	return ""
}
