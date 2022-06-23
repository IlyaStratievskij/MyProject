from django.contrib.auth import logout, login
from django.contrib.auth.decorators import login_required
from django.contrib.auth.models import User
from django.shortcuts import render, redirect
from .forms import *


def loginView(request):
    context = {}
    if request.POST:
        form = AccountAuthenticalForms(request.POST)
        if form.is_valid() :
            email = request.POST['email']
            password = request.POST['password']
            user = authenticate(email=email, password=password)
            login(request, user)
            return redirect('home')
    else:
        form = AccountAuthenticalForms()
    context['form'] = form
    return render(request, 'login.html', context)


def logoutView(request):
    logout(request)
    return redirect("login")


def registerView(request):
    context = {}
    if request.POST:
        form = RegistrationForm(request.POST)
        if form.is_valid():
            user = form.save()
            login(request, user)
            return redirect('home')
    else:
        form = RegistrationForm()
    context['form'] = form
    return render(request, "registrieren.html", context)
