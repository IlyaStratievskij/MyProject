#pragma once
#include <SFML/Graphics.hpp>

sf::View view;

sf::View getPlayerXYForView(float x, float y) {
	float tempX = x;
	float tempY = y;
	if (x < 785) tempX = 785; // ����
	if (x > 2610) tempX = 2610; // �����
	if (y < 350) tempY = 350; // ����
	if (y > 545) tempY = 545; // ���
	view.setCenter(tempX, tempY);

	return view;
}
