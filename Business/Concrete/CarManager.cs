﻿using Business.Abstract;
using Business.Constants;
using Business.ValidationRules.FluentValidation;
using Core.Utilities.Results.Abstract;
using Core.Aspects.Autofac.Validation;
using Core.Utilities.Results.Concrete;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using Core.Aspects.Autofac.Cashing;
using Business.BusinessAspects.Autofac;

namespace Business.Concrete
{
    public class CarManager:ICarService
    {
        ICarDal _carDal;

        public CarManager(ICarDal carDal)
        {
            _carDal = carDal;
        }

        [SecuredOperation("car.add,admin")]
        [CacheRemoveAspect("ICarService.Get")]
        [ValidationAspect(typeof(CarValidator))]
        public IResult Add(Car car)
        {
            _carDal.Add(car);
            return new SuccessResult(Messages.CarAdded);
         
        }

        public IResult Delete(Car car)
        {
            _carDal.Delete(car);
            return new SuccessResult(Messages.CarDelete);
                
        }

        public IDataResult<List<Car>> GetAll()
        {

            if (DateTime.Now.Hour==05)
            {
                return new ErrorDataResult<List<Car>>(Messages.MaintenanceTime);
            }
            return new SuccessDataResult<List<Car>>(_carDal.GetAll(),Messages.CarListed);
        }

        public IDataResult<List<Car>> GetAllByBrandId(int brandId)
        {
            return new SuccessDataResult<List<Car>>(_carDal.GetAll(c => c.BrandId == brandId));
        }

        public IDataResult<List<Car>> GetAllByColorId(int colorId)
        {
            return new SuccessDataResult<List<Car>>(_carDal.GetAll(c => c.ColorId == colorId));
        }

        public IDataResult<List<Car>> GetByDailyPrice(decimal min, decimal max)
        {
            return new SuccessDataResult<List<Car>> (_carDal.GetAll(c => c.DailyPrice >= min && c.DailyPrice <= max));
        }

        public IDataResult<Car> GetById(int carId)
        {
            return new SuccessDataResult<Car>( _carDal.Get(c => c.CarId == carId));
        }

        public IDataResult<List<Car>> GetByModelYear(string year)
        {
            return new SuccessDataResult<List<Car>>(_carDal.GetAll(c => c.ModelYear.Contains(year) == true));
        }

        public IDataResult<List<CarDetailDto>> GetCarDetails()
        {
            return new SuccessDataResult<List<CarDetailDto>>(_carDal.GetCarDetails());
        }
        [SecuredOperation("car.add,admin")]
        [CacheRemoveAspect("ICarService.Update")]
        [ValidationAspect(typeof(CarValidator))]
        public IResult Update(Car car)
        {
            if (car.DailyPrice>0)
            {
                _carDal.Update(car);
                //Console.WriteLine("Araba başarıyla güncellendi.");
                return new SuccessResult(Messages.CarUpdated);
            }
            else
            {
                //Console.WriteLine($"Lütfen fiyat kısmı 0'dan büyük giriniz. Girdiğiniz değer : {car.DailyPrice}");
                return new ErrorResult(Messages.CarPriceInvalid);
            }
        }
    }
}
