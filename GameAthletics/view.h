#pragma once
#include <SFML/Graphics.hpp>

sf::View view;

sf::View getPlayerXYForView(float x, float y) {
	float tempX = x;
	float tempY = y;
	if (x < 785) tempX = 785; // лево
	if (x > 2610) tempX = 2610; // право
	if (y < 350) tempY = 350; // верх
	if (y > 545) tempY = 545; // низ
	view.setCenter(tempX, tempY);

	return view;
}
